import FungibleToken from 0x9a0766d93b6608b7
import Arcane from 0x0095f13a82f1a835
import HeroNFT from 0x0095f13a82f1a835
import AuctionCallbackHandler from 0x0095f13a82f1a835
import FlowTransactionScheduler from 0x8c5303eaa26202d6

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

        if signer.storage.borrow<&ItemManager.Collection>(from: ItemManager.CollectionStoragePath) != nil {
            return
        }else {
            let collection <- ItemManager.createEmptyCollection(nftType: Type<@ItemManager.NFT>())

            // save it to the account
            signer.storage.save(<-collection, to: ItemManager.CollectionStoragePath)

            let collectionCap = signer.capabilities.storage.issue<&ItemManager.Collection>(ItemManager.CollectionStoragePath)
            signer.capabilities.publish(collectionCap, at: ItemManager.CollectionPublicPath)
        }

        // Create a new empty collection
    }
}
