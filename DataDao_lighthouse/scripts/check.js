import { ethers } from "ethers";
import dotenv from "dotenv";
import DataCoinABI from "./abi/DataCoin.js";

const provider = new ethers.JsonRpcProvider("https://eth-sepolia.g.alchemy.com/v2/LFkCz1EjWB-xkvhe5wgip");
const contract = new ethers.Contract(
  "0xA1FB29123489967220b63aE007462B088076F167", // your WDC address
  DataCoinABI,
  provider
);

const MINTER_ROLE = ethers.id("MINTER_ROLE"); // keccak256 hash
const hasRole = await contract.hasRole(MINTER_ROLE, "0xbAFc5BeBeFE176B71A2DC300BCF9E01CA9493423");
console.log("Has MINTER_ROLE?", hasRole);


