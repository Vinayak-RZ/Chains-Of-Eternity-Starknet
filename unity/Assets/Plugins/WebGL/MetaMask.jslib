mergeInto(LibraryManager.library, {
    ConnectMetaMask: function() {
        if (typeof window.ethereum !== 'undefined') {
            window.ethereum.request({ method: 'eth_requestAccounts' })
            .then(function(accounts) {
                Module.SendMessage('AuthCanvas', 'OnMetaMaskConnected', accounts[0]);
            })
            .catch(function(error) {
                Module.SendMessage('AuthCanvas', 'OnMetaMaskError', error.message);
            });
        } else {
            Module.SendMessage('AuthCanvas', 'OnMetaMaskError', 'MetaMask not installed');
        }
    }
});
