// 1. Create global userWalletAddress variable
window.userWalletAddress = null;

// 2. when the browser is ready
window.onload = async (event) => {

  // 2.1 check if ethereum extension is installed
  if (window.ethereum) {

    // 3. create web3 instance
    window.web3 = new Web3(window.ethereum);

  } else {

    // 4. prompt user to install Metamask
    alert("Please install MetaMask or any Ethereum Extension Wallet");
  }

  // 5. check if user is already logged in and update the global userWalletAddress variable
  window.userWalletAddress = window.localStorage.getItem("userWalletAddress");

  // 6. show the user dashboard
  showUserDashboard();
};

// 1. Web3 login function
const loginWithEth = async () => {
  // 1.1 check if there is global window.web3 instance
  if (window.web3) {
    try {
      // 2. get the user's ethereum account - prompts metamask to login
      const selectedAccount = await window.ethereum
        .request({
          method: "eth_requestAccounts",
        })
        .then((accounts) => accounts[0])
        .catch(() => {
          // 2.1 if the user cancels the login prompt
          throw Error("Please select an account");
        });

      // 3. set the global userWalletAddress variable to selected account
      window.userWalletAddress = selectedAccount;

      // 4. store the user's wallet address in local storage
      window.localStorage.setItem("userWalletAddress", selectedAccount);

      // 5. show the user dashboard
      showUserDashboard();

    } catch (error) {
      alert(error);
    }
  } else {
    alert("wallet not found");
  }
};

// 6. when the user clicks the login button run the loginWithEth function
document.querySelector(".login-btn").addEventListener("click", loginWithEth);

// function to show the user dashboard
const showUserDashboard = async () => {

  // if the user is not logged in - userWalletAddress is null
  if (!window.userWalletAddress) {

    // change the page title
    document.title = "Web3 Login";

    // show the login section
    document.querySelector(".login-section").style.display = "flex";

    // hide the user dashboard section
    document.querySelector(".dashboard-section").style.display = "none";

    // return from the function
    return false;
  }

  // change the page title
  document.title = "Web3 Dashboard 🤝";

  // hide the login section
  document.querySelector(".login-section").style.display = "none";

  // show the dashboard section
  document.querySelector(".dashboard-section").style.display = "flex";

  // show the user's wallet address
  showUserWalletAddress();

  // get the user's wallet balance
  // getWalletBalance();
};

// show the user's wallet address from the global userWalletAddress variable
const showUserWalletAddress = () => {
  const walletAddressEl = document.querySelector(".wallet-address");
  walletAddressEl.innerHTML = window.userWalletAddress;
};

// get the user's wallet balance
const getWalletBalance = async () => {
  // check if there is global userWalletAddress variable
  if (!window.userWalletAddress) {
    return false;
  }

  // get the user's wallet balance
  const balance = await window.web3.eth.getBalance(window.userWalletAddress);

  // convert the balance to ether
  document.querySelector(".vpainting-code").innerHTML = web3.utils.fromWei(
    balance,
    "ether"
  );
};


// web3 logout function
const logout = () => {
  // set the global userWalletAddress variable to null
  window.userWalletAddress = null;

  // remove the user's wallet address from local storage
  window.localStorage.removeItem("userWalletAddress");

  // show the user dashboard
  showUserDashboard();
};

// when the user clicks the logout button run the logout function
document.querySelector(".logout-btn").addEventListener("click", logout);