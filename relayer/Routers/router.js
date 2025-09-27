import {express} from 'express';
import { heroCollection } from "../controllers/CollectionHero-test.js";
import { mintHero } from "../controllers/HeroNFT.js";

const router = express.Router();

router.post("/mint-hero", mintHero);

router.post("/hero-collection", heroCollection);
