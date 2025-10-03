import {express} from 'express';
import { heroCollection } from "../controllers/CollectionHero-test.js";
import { mintHero } from "../controllers/HeroNFT.js";
import { updateHero } from "../controllers/UpdateHeroSol.js";
import {mintNFT} from "../controllers/MintNFT.js";
import {emptyCollection} from "../controllers/EmptyCollection.js"
import { AddTokenToMarketPlace } from "../controllers/AddTokenToMarketPlace.js";
import { getLatestNFTs } from "../controllers/topFourListing.js";
import { getUserNFTs } from "../controllers/allUserMintedItems.js";
import { getSpellData } from "../controllers/getSpellData.js";
import { addSpell } from "../controllers/storeSpellinDB.js";
import { BidOnItem } from '../controllers/BidOnItem.js';
import { fetchAndStoreEventsforListing } from "../controllers/marketplace_listed.js"
import { fetchAndStoreEventsforPurchasing } from "../controllers/marketplace_purchased.js"

const router = express.Router();

router.post("/mint-hero", mintHero);

router.post("/hero-collection", heroCollection);
router.post("/update-hero", updateHero);

router.post("/empty-collection", emptyCollection);

router.post("/mint-nft", mintNFT);


router.post("/add-token", AddTokenToMarketPlace);
router.post("/add-spell", addSpell)
router.post("/bid-item", BidOnItem)
router.post("/schedule-auctions", ScheduleAuction)
router.post("/list-auction", ListedAuction);


//get latest 4 nfts data
router.get("/latest-nfts", async (req, res) => {
  try {
    const nfts = await getLatestNFTs(4);
    res.json(nfts);
  } catch (err) {
    res.status(500).json({ success: false, error: err.message });
  }
});

router.post("/user-nfts", async (req, res) => {
  const address = req.body.address;
  try {
    const nfts = await getUserNFTs(address);
    res.json(nfts);
  } catch (err) {
    res.status(500).json({ success: false, error: err.message });
  }
});

router.post("/user-spell", async (req, res) => {
  const address = req.body.address;
  try {
    const nfts = await getSpellData(address);
    console.log(nfts);
    res.json(nfts);
  } catch (err) {
    res.status(500).json({ success: false, error: err.message });
  }
});

//marketplace-listed
router.post("/marketplace-listed", async (req, res) => {
  try {

    const toSaveData = req.body.events[2].data;
    const itemID = toSaveData.itemID;
    const seller = toSaveData.seller;
    const price = toSaveData.price;
    const tokenID = req.body.events[0].data.id;
    console.log("Received itemID:", itemID);
    console.log("Received seller:", seller);
    console.log("Received price:", price);
    console.log("Received tokenID:", tokenID);

    const result = await fetchAndStoreEventsforListing(itemID, seller, price, tokenID)
    console.log("result: ", result)
    res.json({
      success: true,
      ...result
    })
  } catch (err) {
    console.error("Error in /marketplace-listed:", err)
    res.status(500).json({ success: false, error: err.message })
  }
})

//marketplace-purchased
router.post("/marketplace-purchased", async (req, res) => {
  try {
    const latestHeight = await getLatestBlockHeight()
    const { start, end } = req.body

    const startHeight = start || latestHeight - 20
    const endHeight = end || latestHeight

    console.log(`Fetching events from block ${startHeight} → ${endHeight}`)

    const result = await fetchAndStoreEventsforPurchasing(startHeight, endHeight)

    res.json({
      success: true,
      latestHeight,
      ...result
    })
  } catch (err) {
    console.error("Error in /marketplace-purchased:", err)
    res.status(500).json({ success: false, error: err.message })
  }
})


//marketplace-cancelled
//router.post("/marketplace-cancelled", async (req, res) => {
//  try {
//    const latestHeight = await getLatestBlockHeight()
//    const { start, end } = req.body
//
//    const startHeight = start || latestHeight - 20
//    const endHeight = end || latestHeight
//
//    console.log(`Fetching events from block ${startHeight} → ${endHeight}`)
//
//    const result = await fetchAndStoreEventsforPurchasing(startHeight, endHeight)
//
//    res.json({
//      success: true,
//      latestHeight,
//      ...result
//    })
//  } catch (err) {
//    console.error("Error in /marketplace-purchased:", err)
//    res.status(500).json({ success: false, error: err.message })
//  }
//})


export default router;
