
# Arcane: Chains of Eternity

<div align="center">
  <img src="https://github.com/user-attachments/assets/8cc57729-c792-432a-9f85-86da54cdee92" alt="Arcane: Chains of Eternity Logo" width="300">
</div>

---

## 🌌 Overview

**Arcane: Chains of Eternity** is a blockchain-powered fantasy RPG built with **Unity** that blends immersive gameplay with decentralized infrastructure.  
The project was developed for a hackathon and demonstrates how **on-chain assets**, **data monetization**, and **decentralized automation** can enhance modern gaming.

Players venture into a mystical world, casting spells, battling enemies, and trading magical items that live on-chain as NFTs. Each spell and item evolves in value as demand grows, creating a living marketplace powered by decentralized systems.

---

## ✨ Core Features

- **🎮 Unity-based RPG Gameplay**  
  - Built on a **state machine architecture** for both player and enemy AI.
  - Real-time combat, quests, and collectible spell mechanics.

- **🔗 Flow Blockchain Integration**  
  - **Cadence smart contracts** for spells, items, marketplace, and auction house.  
  - **Scheduled transactions** enable automated marketplace settlements, auctions, and periodic rewards (cron-like behavior).

- **🪄 Spell Data Monetization with Lighthouse**  
  - Spells start with equal base value.  
  - Demand-driven growth using **DataCoin**.  
  - Lighthouse SDK for **encryption, access control, and monetization** of spell data.

- **🌐 Identity with ENS**  
  - Players can name their NFTs/items with **ENS names**.  
  - Improves immersion and makes player lookup easier (instead of hex addresses).

- **💸 Marketplace & Auction House**  
  - Fully on-chain marketplace with **scheduled completion** of auctions.  
  - Rewards distributed automatically without manual intervention.

- **📦 Modular Infrastructure**  
  - **Thirdweb & FCL** → Wallet connections.  
  - **Express** → Relayer service.  
  - **Supabase** → Database for off-chain storage.  

---

## Sponsors & Integrations

Our project was made possible with the support of several key blockchain ecosystem providers. Each one plays a unique role in enhancing **Realms of Aether** with scalability, automation, privacy, and personalization.

### Flow: Cadence Smart Contracts & Scheduled Transactions

* We leverage **Flow** for building Cadence smart contracts.
* Flow’s **consumer-based chain architecture** makes it ideal for handling the large player base we are targeting.
* **Scheduled Transactions** power our **Marketplace and Auction House**, allowing automated completion of auctions, timed rewards, and periodic distribution of resources—similar to cron jobs but fully on-chain.
* This ensures smooth, scalable, and player-friendly experiences without requiring constant manual intervention.

### Lighthouse: Spell Data Encryption & Monetization

* We integrate **Lighthouse SDK** to securely **encrypt spell data** and manage controlled access.
* Players can share spells with others, while maintaining **ownership rights** and the ability to **grant or revoke access**.
* Spells are monetized using a **data coin** model:

  * Each spell starts with the same baseline value.
  * As demand grows, the value increases based on the number of data coins linked to it.
* This creates a new **player-driven economy for knowledge and spell crafting**, enabling creators to be rewarded as their spells gain popularity.

### ENS: Personalized NFT & Item Naming

* To make in-game identity more immersive, we use **Ethereum Name Service (ENS)**.
* Players can **name their NFTs, items, or characters** with human-readable ENS names instead of long hex wallet addresses.
* Benefits:

  * Stronger emotional connection between players and their characters/items.
  * Easier lookup and discovery of players in the game’s ecosystem.
  * Seamless integration with the wider Ethereum ecosystem, where ENS is widely supported.

---
## 🛠 Tech Stack

- **Game Engine**: Unity (C#)
- **Wallet Connections**: Thirdweb, FCL (Flow Client Library)
- **Smart Contracts**: Flow Cadence
- **Relayer**: Node.js (Express)
- **Database**: Supabase
- **Encryption & Access**: Lighthouse SDK
- **Identity**: ENS

---

## 🚀 Getting Started

### Prerequisites
- [Unity 2023.3+](https://unity.com/)
- [Node.js 18+](https://nodejs.org/)
- [Flow CLI](https://developers.flow.com/tools/flow-cli)
- [Supabase account](https://supabase.com/)
- [Lighthouse SDK](https://docs.lighthouse.storage/)

### Setup

1. **Clone the repository**  
   ```bash
   git clone https://github.com/<your-org>/arcane-chains-of-eternity.git
   cd arcane-chains-of-eternity


2. **Unity Project**

   * Open the project in Unity Hub.
   * Load the main scene from `Assets/Scenes/`.

3. **Relayer Service**

   ```bash
   cd relayer
   npm install
   npm start
   ```

4. **Flow Contracts**
   Deploy Cadence contracts:

   ```bash
   flow project deploy
   ```

5. **Database (Supabase)**

   * Configure your `.env` with Supabase credentials.
   * Run migrations if needed.

---

## 📂 Project Structure

```
Arcane-Chains-of-Eternity/
│
├── Assets/                # Unity game assets & scripts
│   ├── Scripts/           # Game logic (C#, state machines, integrations)
│   ├── Scenes/            # Unity scenes
│   └── Prefabs/           # Prefab assets
│
├── contracts/             # Flow Cadence smart contracts
├── relayer/               # Express.js relayer service
├── db/                    # Supabase integration configs
└── README.md
```

---

## 🌍 Vision

Arcane: Chains of Eternity isn’t just a game — it’s an experiment in combining **gameplay depth** with **decentralized systems**.
Our goal is to create a world where spells, items, and player identities hold **real value**, controlled and owned by the community.

---

## 🤝 Contributors

* Built by a passionate team during a hackathon.
* Open for collaboration and future development!

---

## 📜 License

This project is licensed under the MIT License.

```
