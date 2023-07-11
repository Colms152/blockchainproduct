//import Web3 from 'web3';
//import Color from '../abis/Color.json'

const Web3 = require('web3')
const Color = require('../../../storage/output/contracts/Color.json')
//Down grade dependancy version in package
var Tx = require('ethereumjs-tx')

var express = require('express');
var app = express();
const bodyParser = require('body-parser')

// sets port 8080 to default or unless otherwise specified in the environment
app.set('port', 8080);

// --> 11)  Mount the body-parser middleware  here
app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: false }));

let componentWillMount = async function() { 
    await loadWeb3()
    await loadBlockchainData()
        
};

var web3var

var objvars = { 
    account: '',
    contract: null,
    totalSupply: 0,
    colors: [],
    networkData: null
  };
var objaccount1 = {
    address: '',
    pKey: ''
}  

// Constants
const PORT = 8545;
const HOST = '0.0.0.0';

async function loadWeb3() {    
    web3var = new Web3('http://ganache-cli:8545')    
  }

async function loadBlockchainData(){
    const web3 = web3var
    //Load account
    const accounts = await web3.eth.getAccounts()
    console.log("Accounts " + accounts)
    objvars.account = accounts[0]
    //this.setState({account: accounts[0]})
    const networkID = await web3.eth.net.getId()
    console.log('*****' + networkID)
       objvars.networkData = Color.networks[networkID]

    if(objvars.networkData) {
      console.log('*****' + objvars.networkData.address)
      const abi = Color.abi
      const address = objvars.networkData.address
      var myContract = new web3.eth.Contract(abi, address);
      objvars.contract = myContract
      //this.setState({ myContract })
      const totalSupply = await objvars.contract.methods.totalSupply().call()
      objvars.totalSupply = totalSupply
      
      //this.setState({totalSupply})
      //Load Colors
      objvars.colors = []
      for (var i =1; i <= totalSupply; i++){
        const color = await myContract.methods.colors(i-1).call()
        
        objvars.colors.push(color) 
        //this.setState({
        //  colors: [...this.state.colors, color]
        //})
    }
    console.log(objvars.colors)
    } else {
      console.log('****Not Useable****')
    }
    
  }

  mint = (color) => {
    console.log(objvars)
    console.log("form account:" + objvars.account)
    objvars.contract.methods.mint(color).send({ from: objvars.account})
    .once('receipt', (receipt) => {
      
    })

  }
  
  mint2 = (color) => {
    web3var.eth.getTransactionCount(objaccount1.address, (err, txCount) => {

      const txObject = {
        nonce:    web3var.utils.toHex(txCount),
        gasLimit: web3var.utils.toHex(800000), // Raise the gas limit to a much higher amount
        gasPrice: web3var.utils.toHex(web3var.utils.toWei('10', 'gwei')),
        to: objvars.networkData.address,
        data: objvars.contract.methods.mint(color).encodeABI()
      }
    
      const tx = new Tx(txObject)
      tx.sign(objaccount1.pKey)
    
      const serializedTx = tx.serialize()
      const raw = '0x' + serializedTx.toString('hex')
    
      web3var.eth.sendSignedTransaction(raw, (err, txHash) => {
        console.log('err:', err, 'txHash:', txHash)
        // Use this txHash to find the contract on Etherscan!
      })
    })
    
    
  }

//end point
app.get("/", function(req, res) {
    componentWillMount()

    res.send('Loading')
  });
  app.get("/acca/:add/:pk", function(req, res) {
    objaccount1.address = req.params.add;
    objaccount1.pKey = Buffer.from(req.params.pk, 'hex');    
      res.send('Data entered')
    
  });
app.get("/view", function(req, res) {
    componentWillMount().then(
        res.send(objvars.colors)
    )
  });
app.get("/mint/:colorid", function(req, res) {
    mint2(req.params.colorid)
    componentWillMount().then(
      res.send(objvars.colors)
  )
  });  



// listen for requests :)
var listener = app.listen(8080, function () {
    console.log('Your app is listening on port ' + listener.address().port);
  });  