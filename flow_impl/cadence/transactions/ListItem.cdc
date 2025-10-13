import ItemManager from 0x0095f13a82f1a835 //from 0x3d895199cfc42ff5
import MarketPlace2 from 0x0095f13a82f1a835 //from 0x3d895199cfc42ff5
import NonFungibleToken from 0x631e88ae7f1d7c20 //from 0x631e88ae7f1d7c20

transaction(tokenID: UInt64, price: UFix64) {
    let withdrawRef: auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}
    prepare(signer: auth(Storage , BorrowValue) &Account) {
        self.withdrawRef = signer.storage.borrow<auth(NonFungibleToken.Withdraw) &{NonFungibleToken.Collection}>(from: ItemManager.CollectionStoragePath)
            ?? panic("Missing ItemManager collection")

        // Withdraw NFT from seller's collection (this requires signer's withdraw auth)
        let nft <- self.withdrawRef.withdraw(withdrawID: tokenID)

        // Pass the NFT resource and signer.address into contract
        MarketPlace2.listItem(nft: <- nft, price: price, seller: signer.address)
    }
}