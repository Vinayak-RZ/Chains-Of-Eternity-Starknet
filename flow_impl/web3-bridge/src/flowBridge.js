import * as fcl from "@onflow/fcl"

// ---- Configure FCL for Testnet (Cadence 1.0 ready) ----
fcl.config()
  .put("app.detail.title", "Hero Game")
  .put("flow.network", "testnet")
  .put("accessNode.api", "https://rest-testnet.onflow.org")
  .put("discovery.wallet", "https://fcl-discovery.onflow.org/testnet/authn")

let unityInstanceRef = null;

function setUnityInstance(instance) {
  unityInstanceRef = instance;
}

function sendToUnity(method, payload = "") {
  try {
    if (unityInstanceRef) {
      // 👇 updated: target Web3AuthManager
      unityInstanceRef.SendMessage("Web3AuthManager", method, String(payload ?? ""));
    } else {
      console.log("[FlowBridge->Unity]", method, payload);
    }
  } catch (e) {
    console.warn("SendMessage failed:", e);
  }
}


async function authn() {
  // Opens wallet; resolves when user has an authenticated session
  await fcl.authenticate()
  const user = await fcl.currentUser().snapshot()
  return user
}

// ---- Public API (exposed on window.FlowBridge) ----
async function connectFlow() {
  try {
    const user = await authn()
    sendToUnity("OnFlowWalletConnected", user?.addr ?? "")
  } catch (e) {
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function disconnectFlow() {
  try {
    await fcl.unauthenticate()
    sendToUnity("OnWalletDisconnected", "")
  } catch (e) {
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

// Example transaction: create empty Hero collection (no args)
async function createHeroCollection() {
  const cadence = `
    import HeroNFT from 0x0095f13a82f1a835

    transaction {
      prepare(signer: auth(BorrowValue, IssueStorageCapabilityController, PublishCapability, SaveValue, UnpublishCapability) &Account) {
        if signer.storage.borrow<&HeroNFT.Collection>(from: HeroNFT.CollectionStoragePath) != nil {
          return
        }
        let collection <- HeroNFT.createEmptyCollection(nftType: Type<@HeroNFT.NFT>())
        signer.storage.save(<-collection, to: HeroNFT.CollectionStoragePath)
        let collectionCap = signer.capabilities.storage.issue<&HeroNFT.Collection>(HeroNFT.CollectionStoragePath)
        signer.capabilities.publish(collectionCap, at: HeroNFT.CollectionPublicPath)
      }
    }
  `
  try {
    // Use the current wallet user as proposer/payer/authorizations
    const authz = fcl.authz // NOTE: with FCL, proposer/payer/authz default to current user; explicit is fine too
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })
    console.log(txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    // Wait until sealed
    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function bidOnItem(listingID, paymentAmount) {
  const cadence = `
    import FungibleToken from 0x9a0766d93b6608b7
    import FlowToken from 0x7e60df042a9c0868
    import NonFungibleToken from 0x631e88ae7f1d7c20
    import ItemManager from 0x0095f13a82f1a835   // replace if different
    import AuctionHouse from 0x0095f13a82f1a835   // replace with marketplace address
    transaction(listingID: UInt64, paymentAmount: UFix64) {
        let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}
        let collectionRef: &ItemManager.Collection
        prepare(buyer: auth(Storage, BorrowValue) &Account) {
            self.vaultRef = buyer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/flowTokenVault)
            ?? panic("Missing FlowToken vault in buyer account. Please create & link one.")
            // 3) Withdraw the paymentAmount (should be >= listing price; contract will refund any extra)
            let payment <- self.vaultRef.withdraw(amount: paymentAmount)
            self.collectionRef = buyer.storage.borrow<&ItemManager.Collection>(
                from: ItemManager.CollectionStoragePath // Assuming this exists; if not, replace with the actual StoragePath, e.g., /storage/ItemManagerCollection
            ) ?? panic("Missing ItemManager collection in buyer account. Please create & link one.")
            // 4) Call marketplace purchase. Buyer address passed so contract can route refunds, deposits, etc.
            AuctionHouse.placeBid(
                listingID: listingID,
                bidder: buyer.address,
                payment: <-payment
            )
        }
        execute {
        log("Purchase transaction executed — check marketplace events for details.")
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(listingID.toString(), t.UInt64),
        arg(paymentAmount.toFixed(2), t.UFix64) // assumes JS number, formats to string like "10.00"
      ],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })
    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function buyItem(listingID, paymentAmount) {
  const cadence = `
    import FungibleToken from 0x9a0766d93b6608b7
    import FlowToken from 0x7e60df042a9c0868
    import NonFungibleToken from 0x631e88ae7f1d7c20 
    import ItemManager from 0x0095f13a82f1a835   // replace if different
    import MarketPlace2 from 0x0095f13a82f1a835   // replace with marketplace address
    transaction(listingID: UInt64, paymentAmount: UFix64) {
        let vaultRef: auth(FungibleToken.Withdraw) &{FungibleToken.Vault}
        let collectionRef: &ItemManager.Collection
        prepare(buyer: auth(Storage, BorrowValue) &Account) {
            self.vaultRef = buyer.storage.borrow<auth(FungibleToken.Withdraw) &{FungibleToken.Vault}>(from: /storage/flowTokenVault)
            ?? panic("Missing FlowToken vault in buyer account. Please create & link one.")
            // 3) Withdraw the paymentAmount (should be >= listing price; contract will refund any extra)
            let payment <- self.vaultRef.withdraw(amount: paymentAmount)
            self.collectionRef = buyer.storage.borrow<&ItemManager.Collection>(
                from: ItemManager.CollectionStoragePath // Assuming this exists; if not, replace with the actual StoragePath, e.g., /storage/ItemManagerCollection
            ) ?? panic("Missing ItemManager collection in buyer account. Please create & link one.")
            // 4) Call marketplace purchase. Buyer address passed so contract can route refunds, deposits, etc.
            MarketPlace2.purchase(
                listingID: listingID,
                buyer: buyer.address,
                buyerCollection: self.collectionRef,
                payment: <-payment
            )
        }
        execute {
        log("Purchase transaction executed — check marketplace events for details.")
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(listingID.toString(), t.UInt64),
        arg(paymentAmount.toFixed(2), t.UFix64) // formats JS number -> "10.00"
      ],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function setupArcaneTokenAccount() {
  const cadence = `
    import FungibleToken from 0x9a0766d93b6608b7
    import Arcane from 0x0095f13a82f1a835

    transaction () {

        prepare(signer: auth(BorrowValue, IssueStorageCapabilityController, PublishCapability, SaveValue) &Account) {

            // Return early if the account already stores a Arcane Vault
            if signer.storage.borrow<&Arcane.Vault>(from: Arcane.VaultStoragePath) != nil {
                return
            }

            let vault <- Arcane.createEmptyVault(vaultType: Type<@Arcane.Vault>())

            // Create a new Arcane Vault and put it in storage
            signer.storage.save(<-vault, to: Arcane.VaultStoragePath)

            // Create a public capability to the Vault that exposes the Vault interfaces
            let vaultCap = signer.capabilities.storage.issue<&Arcane.Vault>(
                Arcane.VaultStoragePath
            )
            signer.capabilities.publish(vaultCap, at: Arcane.VaultPublicPath)
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [], // no arguments
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function createNFTCollection() {
  const cadence = `
    import ItemManager from 0x0095f13a82f1a835
    import NonFungibleToken from 0x631e88ae7f1d7c20

    transaction {

        prepare(signer: auth(BorrowValue, IssueStorageCapabilityController, PublishCapability, SaveValue, UnpublishCapability) &Account) {

            // Return early if the account already has a collection
            if signer.storage.borrow<&ItemManager.Collection>(from: ItemManager.CollectionStoragePath) != nil {
                return
            }

            // Create a new empty collection
            let collection <- ItemManager.createEmptyCollection(nftType: Type<@ItemManager.NFT>())

            // save it to the account
            signer.storage.save(<-collection, to: ItemManager.CollectionStoragePath)

            let collectionCap = signer.capabilities.storage.issue<&ItemManager.Collection>(ItemManager.CollectionStoragePath)
            signer.capabilities.publish(collectionCap, at: ItemManager.CollectionPublicPath)
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [], // no arguments
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function listOnAuction(delaySeconds, priority, executionEffort, tokenID, price) {
  const cadence = `
    import FlowTransactionScheduler from 0x8c5303eaa26202d6
    import FlowToken from 0x7e60df042a9c0868
    import FungibleToken from 0x9a0766d93b6608b7
    import AuctionHouse from 0x0095f13a82f1a835
    import AuctionCallbackHandler from 0x0095f13a82f1a835
    import NonFungibleToken from 0x631e88ae7f1d7c20
    import ItemManager from 0x0095f13a82f1a835

    /// Schedule an increment of the Counter with a relative delay in seconds
    transaction(
        delaySeconds: UFix64,
        priority: UInt8,
        executionEffort: UInt64,
        tokenID: UInt64,
        price: UFix64
    ) {
        let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}
        prepare(signer: auth(Storage, Capabilities) &Account) {
            self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)
                ?? panic("Missing ItemManager collection")

            // Withdraw NFT from seller's collection (this requires signer's withdraw auth)
            let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)

            // Pass the NFT resource and signer.address into contract
            let future = getCurrentBlock().timestamp + delaySeconds
            let listId: UInt64 = AuctionHouse.listItem(nft: <- nft, basePrice: price, seller: signer.address, endTime: future)
            let transactionData = AuctionCallbackHandler.loradata(listingId: listId)

            let pr = priority == 0
                ? FlowTransactionScheduler.Priority.High
                : priority == 1
                    ? FlowTransactionScheduler.Priority.Medium
                    : FlowTransactionScheduler.Priority.Low

            let est = FlowTransactionScheduler.estimate(
                data: transactionData,
                timestamp: future,
                priority: pr,
                executionEffort: executionEffort
            )

            assert(
                est.timestamp != nil || pr == FlowTransactionScheduler.Priority.Low,
                message: est.error ?? "estimation failed"
            )

            let vaultRef = signer.storage
                .borrow<auth(FungibleToken.Withdraw) &FlowToken.Vault>(from: /storage/flowTokenVault)
                ?? panic("missing FlowToken vault")
            let fees <- vaultRef.withdraw(amount: est.flowFee ?? 0.0) as! @FlowToken.Vault

            let handlerCap = signer.capabilities.storage
                .issue<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>(/storage/AuctionCallbackHandler)

            let receipt <- FlowTransactionScheduler.schedule(
                handlerCap: handlerCap,
                data: transactionData,
                timestamp: future,
                priority: pr,
                executionEffort: executionEffort,
                fees: <-fees
            )

            log("Scheduled transaction id: ".concat(receipt.id.toString()).concat(" at ").concat(receipt.timestamp.toString()))
            
            destroy receipt
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(delaySeconds.toFixed(2), t.UFix64),  // e.g. 10.0
        arg(priority.toString(), t.UInt8),       // 0=High, 1=Medium, 2=Low
        arg(executionEffort.toString(), t.UInt64),
        arg(tokenID.toString(), t.UInt64),
        arg(price.toFixed(2), t.UFix64)
      ],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function initializeAuctionScheduler() {
  const cadence = `
    import AuctionCallbackHandler from 0x0095f13a82f1a835
    import FlowTransactionScheduler from 0x8c5303eaa26202d6

    transaction() {
        prepare(signer: auth(Storage, Capabilities) &Account) {
          
            if signer.storage.borrow<&AnyResource>(from: /storage/AuctionCallbackHandler) == nil {
                let handler <- AuctionCallbackHandler.createHandler()
                signer.storage.save(<-handler, to: /storage/AuctionCallbackHandler)
            }
            let _ = signer.capabilities.storage
                .issue<auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}>(/storage/AuctionCallbackHandler)
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [], // no arguments
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function listItemOnMarketplace(tokenID, price) {
  const cadence = `
    import ItemManager from 0x0095f13a82f1a835
    import MarketPlace2 from 0x0095f13a82f1a835
    import NonFungibleToken from 0x631e88ae7f1d7c20

    transaction(tokenID: UInt64, price: UFix64) {
        let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}
        prepare(signer: auth(Storage , BorrowValue) &Account) {
            self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)
                ?? panic("Missing ItemManager collection")

            // Withdraw NFT from seller's collection
            let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)

            // Pass the NFT resource and seller address to the marketplace
            MarketPlace2.listItem(nft: <- nft, price: price, seller: signer.address)
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [
        arg(tokenID.toString(), t.UInt64),
        arg(price.toFixed(2), t.UFix64)  // Ensure string with 2 decimals
      ],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId);

    const txDetails = await fcl.tx(txId).onceSealed();
    sendToUnity("OnFlowTxSealed", JSON.stringify(txDetails))

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}


// Keep a simple getter for current user
async function getFlowUser() {
  try {
    const user = await fcl.currentUser().snapshot()
    sendToUnity("OnFlowUser", JSON.stringify(user || {}))
  } catch (e) {
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}

async function initializePlayerAccount() {
  const cadence = `
    import FungibleToken from 0x9a0766d93b6608b7
    import Arcane from 0x0095f13a82f1a835
    import HeroNFT from 0x0095f13a82f1a835
    import AuctionCallbackHandler from 0x0095f13a82f1a835
    import FlowTransactionScheduler from 0x8c5303eaa26202d6
    import ItemManager from 0x0095f13a82f1a835

    transaction() {

        prepare(
            signer: auth(
                BorrowValue,
                IssueStorageCapabilityController,
                PublishCapability,
                SaveValue,
                UnpublishCapability,
                Storage,
                Capabilities
            ) &Account
        ) {

            //
            // 1. Initialize Arcane Vault
            //
            if signer.storage.borrow<&Arcane.Vault>(from: Arcane.VaultStoragePath) == nil {
                let vault <- Arcane.createEmptyVault(vaultType: Type<@Arcane.Vault>())
                signer.storage.save(<-vault, to: Arcane.VaultStoragePath)

                let vaultCap = signer.capabilities.storage.issue<&Arcane.Vault>(
                    Arcane.VaultStoragePath
                )
                signer.capabilities.publish(vaultCap, at: Arcane.VaultPublicPath)
            }

            //
            // 2. Initialize HeroNFT Collection
            //
            if signer.storage.borrow<&HeroNFT.Collection>(from: HeroNFT.CollectionStoragePath) == nil {
                let collection <- HeroNFT.createEmptyCollection(nftType: Type<@HeroNFT.NFT>())
                signer.storage.save(<-collection, to: HeroNFT.CollectionStoragePath)

                let collectionCap = signer.capabilities.storage.issue<&HeroNFT.Collection>(
                    HeroNFT.CollectionStoragePath
                )
                signer.capabilities.publish(collectionCap, at: HeroNFT.CollectionPublicPath)
            }

            //
            // 3. Initialize AuctionCallbackHandler
            //
            if signer.storage.borrow<&AnyResource>(from: /storage/AuctionCallbackHandler) == nil {
                let handler <- AuctionCallbackHandler.createHandler()
                signer.storage.save(<-handler, to: /storage/AuctionCallbackHandler)
            }

            let _ = signer.capabilities.storage.issue<
                auth(FlowTransactionScheduler.Execute) &{FlowTransactionScheduler.TransactionHandler}
            >(/storage/AuctionCallbackHandler)

            //
            // 4. Initialize ItemManager Collection
            //
            if signer.storage.borrow<&ItemManager.Collection>(from: ItemManager.CollectionStoragePath) == nil {
                let collection <- ItemManager.createEmptyCollection(nftType: Type<@ItemManager.NFT>())
                signer.storage.save(<-collection, to: ItemManager.CollectionStoragePath)

                let collectionCap = signer.capabilities.storage.issue<&ItemManager.Collection>(ItemManager.CollectionStoragePath)
                signer.capabilities.publish(collectionCap, at: ItemManager.CollectionPublicPath)
            }
        }
    }
  `

  try {
    const txId = await fcl.mutate({
      cadence,
      args: (arg, t) => [],
      proposer: fcl.authz,
      payer: fcl.authz,
      authorizations: [fcl.authz],
      limit: 9999
    })

    console.log("Tx submitted:", txId)
    sendToUnity("OnFlowTxSubmitted", txId)

    await fcl.tx(txId).onceSealed()
    sendToUnity("OnFlowTxSealed", txId)

  } catch (e) {
    console.error("Transaction failed:", e)
    sendToUnity("OnFlowError", e?.message ?? String(e))
  }
}


// Expose to window for Unity .jslib to access
if (typeof window !== "undefined") {
  window.FlowBridge = {
    setUnityInstance,
    connectFlow,
    disconnectFlow,
    createHeroCollection,
    getFlowUser,
    initializeAuctionScheduler,
    bidOnItem,
    buyItem,
    setupArcaneTokenAccount,
    createNFTCollection,
    listOnAuction,
    listItemOnMarketplace,
    initializePlayerAccount
  }
}