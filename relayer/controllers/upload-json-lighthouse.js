// lighthouseController.js
import dotenv from "dotenv";
import axios from "axios";
import lighthouse from "@lighthouse-web3/sdk";
import { addSpell } from "./storeSpellinDB.js";
dotenv.config();

/**
 * Upload encrypted JSON to Lighthouse
 */
export const uploadJsonToLighthouse = async (req, res) => {
  try {
    const { jsonData, publicKey, signedMessage } = req.body;
    console.log("Received upload request:", { jsonData, publicKey, signedMessage });
    const checkSpell = await addSpell()
    if (checkSpell.success){
      const response = await lighthouse.textUploadEncrypted(
        checkSpell.data,
        process.env.API_KEY, // must be set in .env
        publicKey,
        signedMessage
      );
      console.log("Lighthouse upload response:", response);
      return res.status(200).json({
        success: true,
        data: response,
      });
    } else {
      return res.json({
        success: false,
        message: "Spell already exits!"
      })
    }
  } catch (error) {
    return res.status(500).json({
      success: false,
      message: error.message,
    });
  }
};

/**
 * Apply zkTLS conditions for a CID
 */
export const applyZkConditions = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage, conditions } = req.body;

    const nodeId = [1, 2, 3, 4, 5];
    const nodeUrl = nodeId.map(
      (id) => `https://encryption.lighthouse.storage/api/setZkConditions/${id}`
    );

    const config = {
      method: "post",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${signedMessage}`,
      },
    };

    const apidata = { address: publicKey, cid, conditions };

    const results = [];
    for (const url of nodeUrl) {
      const resp = await axios({ url, data: apidata, ...config });
      results.push(resp.data);
    }

    return res.json({ success: true, data: results });
  } catch (err) {
    return res.status(500).json({ success: false, message: err.message });
  }
};

/**
 * Verify zkTLS proof + decrypt JSON file
 */
export const verifyAndDecrypt = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage } = req.body;

    const nodeId = [1, 2, 3, 4, 5];
    const nodeUrl = nodeId.map(
      (id) =>
        `https://encryption.lighthouse.storage/api/verifyZkConditions/${id}`
    );

    const config = {
      method: "post",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${signedMessage}`,
      },
    };

    const apidata = { address: publicKey, cid, proof };

    const shards = [];
    for (const url of nodeUrl) {
      const resp = await axios({ url, data: apidata, ...config });
      shards.push(resp.data.payload);
    }

    const { masterKey, error } = await lighthouse.recoverKey(shards);
    if (error) throw new Error(error);

    const decrypted = await lighthouse.decryptFile(cid, masterKey);

    const jsonData = JSON.parse(Buffer.from(decrypted).toString());

    return res.json({ success: true, data: jsonData });
  } catch (err) {
    return res.status(500).json({ success: false, message: err.message });
  }
};

Got it ✅ — since you’re shifting from frontend React → backend (Node + Express), here’s how you can restructure things:

🔑 Key changes when moving to backend:

Frontend: only signs the message using the user’s wallet (Metamask).

Backend: receives { cid, publicKey, signedMessage }, checks permissions in your DB, then uses Lighthouse SDK to fetch key + decrypt JSON.

Frontend never directly calls Lighthouse SDK anymore — only your backend.

Here’s a backend version of your decrypt function (hooked into Express):

// controllers/lighthouseController.js
import dotenv from "dotenv";
import lighthouse from "@lighthouse-web3/sdk";
import { pool } from "../db.js"; // your Supabase pool

dotenv.config();

/**
 * Decrypt JSON from Lighthouse (backend)
 * Expects: { cid, publicKey, signedMessage }
 */
export const decryptJsonFromLighthouse = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage } = req.body;

    if (!cid || !publicKey || !signedMessage) {
      return res.status(400).json({
        success: false,
        message: "cid, publicKey and signedMessage are required",
      });
    }

    // 1️⃣ DB check: does this address own/borrow/buy this spell?
    const { rows: ownership } = await pool.query(
      `SELECT role 
       FROM SpellOwnership 
       WHERE spell_cid = $1 AND address = $2`,
      [cid, publicKey]
    );

    if (ownership.length === 0) {
      return res.status(403).json({
        success: false,
        message: "Access denied: this address has no rights for this spell",
      });
    }

    // 2️⃣ Ask Lighthouse for encryption key
    const fileEncryptionKey = await lighthouse.fetchEncryptionKey(
      cid,
      publicKey,
      signedMessage
    );

    if (!fileEncryptionKey?.data?.key) {
      return res.status(403).json({
        success: false,
        message: "Lighthouse denied access (no encryption key returned)",
      });
    }

    // 3️⃣ Decrypt file
    const decrypted = await lighthouse.decryptFile(
      cid,
      fileEncryptionKey.data.key
    );

    const jsonData = await decrypted.Text()

    const data = JSON.parse(jsonData)
    // 4️⃣ Return JSON
    return res.json({
      success: true,
      role: ownership[0].role,
      data: data,
    });
  } catch (err) {
    console.error("decryptJsonFromLighthouse error:", err);
    return res.status(500).json({ success: false, message: err.message });
  }
};

export const shareFileAccess = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage, receivers } = req.body;

    if (!cid || !publicKey || !signedMessage || !receivers?.length) {
      return res.status(400).json({
        success: false,
        message: "cid, publicKey, signedMessage and receivers[] are required",
      });
    }

    // 1️⃣ Call Lighthouse to share access
    const shareResponse = await lighthouse.shareFile(
      publicKey,
      receivers, // array of receiver wallet addresses
      cid,
      signedMessage
    );

    // 2️⃣ Save each receiver into SpellOwnership (role = BORROWER by default)
    for (const receiver of receivers) {
      await pool.query(
        `INSERT INTO SpellOwnership (address, role, spell_cid)
         VALUES ($1, 'BORROWER', $2)
         ON CONFLICT DO NOTHING`, // prevents duplicate entries
        [receiver, cid]
      );
    }

    return res.json({
      success: true,
      data: shareResponse.data,
    });
  } catch (err) {
    console.error("shareFileAccess error:", err);
    return res.status(500).json({ success: false, message: err.message });
  }
};

/**
 * Revoke file access
 * Expects: { cid, publicKey, signedMessage, revokeFrom }
 */
export const revokeFileAccess = async (req, res) => {
  try {
    const { cid, publicKey, signedMessage, revokeFrom } = req.body;

    if (!cid || !publicKey || !signedMessage || !revokeFrom?.length) {
      return res.status(400).json({
        success: false,
        message: "cid, publicKey, signedMessage and revokeFrom[] are required",
      });
    }

    // 1️⃣ Call Lighthouse revoke
    const revokeResponse = await lighthouse.revokeFileAccess(
      publicKey,
      revokeFrom, // array of wallet addresses to revoke
      cid,
      signedMessage
    );

    // 2️⃣ Remove from SpellOwnership
    await pool.query(
      `DELETE FROM SpellOwnership 
       WHERE spell_cid = $1 AND address = ANY($2::text[])`,
      [cid, revokeFrom]
    );

    return res.json({
      success: true,
      data: revokeResponse.data,
    });
  } catch (err) {
    console.error("revokeFileAccess error:", err);
    return res.status(500).json({ success: false, message: err.message });
  }
};

import pool from "../db.js";

/**
 * Get all spells owned/borrowed/bought by an account
 * Expects: /api/spells/:address
 */
export const getAccountSpells = async (req, res) => {
  try {
    const { address } = req.params;

    if (!address) {
      return res.status(400).json({ success: false, message: "Address required" });
    }

    const result = await pool.query(
      `SELECT spell_cid, role, created_at
       FROM SpellOwnership
       WHERE address = $1`,
      [address]
    );

    return res.json({
      success: true,
      count: result.rows.length,
      spells: result.rows,
    });
  } catch (err) {
    console.error("getAccountSpells error:", err);
    return res.status(500).json({ success: false, message: err.message });
  }
};

