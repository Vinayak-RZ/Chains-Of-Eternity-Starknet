import {express} from 'express';
import { heroCollection } from "../controllers/CollectionHero-test.js";
import { mintHero } from "../controllers/HeroNFT.js";
import { emptyCollection } from "../controllers/EmptyCollection.js";
import { mintNFT } from "../controllers/MintNFT.js";


const router = express.Router();

router.post("/mint-hero", mintHero);

router.post("/hero-collection", heroCollection);

router.post("/empty-collection", emptyCollection);

router.post("/mint-nft", mintNFT);
