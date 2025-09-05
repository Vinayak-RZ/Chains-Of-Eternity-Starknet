mergeInto(LibraryManager.library, {
  FlowBridge_SetUnityInstance: function(instancePtr) {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.setUnityInstance(instancePtr);
    }
  },
  FlowBridge_ConnectFlow: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.connectFlow();
    }
  },
  FlowBridge_DisconnectFlow: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.disconnectFlow();
    }
  },
  FlowBridge_CreateHeroCollection: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.createHeroCollection();
    }
  },
  FlowBridge_BidOnItem: function(listingIDPtr, paymentAmountPtr) {
    if (typeof window.FlowBridge !== "undefined") {
      var listingID = UTF8ToString(listingIDPtr);
      var paymentAmount = UTF8ToString(paymentAmountPtr);
      window.FlowBridge.bidOnItem(listingID, parseFloat(paymentAmount));
    }
  },
  FlowBridge_BuyItem: function(listingIDPtr, paymentAmountPtr) {
    if (typeof window.FlowBridge !== "undefined") {
      var listingID = UTF8ToString(listingIDPtr);
      var paymentAmount = UTF8ToString(paymentAmountPtr);
      window.FlowBridge.buyItem(listingID, parseFloat(paymentAmount));
    }
  },
  FlowBridge_SetupArcaneTokenAccount: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.setupArcaneTokenAccount();
    }
  },
  FlowBridge_InitializeAuctionScheduler: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.initializeAuctionScheduler();
    }
},
  FlowBridge_GetFlowUser: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.getFlowUser();
    }
  },
  FlowBridge_CreateNFTCollection: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.createNFTCollection();
    }
  },
  FlowBridge_ListOnAuction: function(delaySecondsPtr, priorityPtr, executionEffortPtr, tokenIDPtr, pricePtr) {
    if (typeof window.FlowBridge !== "undefined") {
      var delaySeconds = parseFloat(UTF8ToString(delaySecondsPtr));
      var priority = parseInt(UTF8ToString(priorityPtr));
      var executionEffort = parseInt(UTF8ToString(executionEffortPtr));
      var tokenID = parseInt(UTF8ToString(tokenIDPtr));
      var price = parseFloat(UTF8ToString(pricePtr));
      window.FlowBridge.listOnAuction(delaySeconds, priority, executionEffort, tokenID, price);
    }
  },
  FlowBridge_InitializePlayerAccount: function() {
    if (typeof window.FlowBridge !== "undefined") {
      window.FlowBridge.initializePlayerAccount();
    }
  },
  FlowBridge_ListItemOnMarketplace: function(tokenIDPtr, pricePtr) {
    if (typeof window.FlowBridge !== "undefined") {
      var tokenID = parseInt(UTF8ToString(tokenIDPtr));
      var price = parseFloat(UTF8ToString(pricePtr));
      window.FlowBridge.listItemOnMarketplace(tokenID, price);
    }
  }
});
