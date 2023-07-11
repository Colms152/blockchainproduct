// New file to merge swager 2.0 and previous server file
//import Web3 from 'web3';
//import Color from '../abis/Color.json'

const Web3 = require('web3')
const Color = require('../../../storage/output/contracts/Color.json')
//Down grade dependancy version in package
var Tx = require('ethereumjs-tx')

var express = require('express');
var app = express();
const swaggerJsDoc = require('swagger-jsdoc');
const swaggerUi = require('swagger-ui-express');

// sets port 8080 to default or unless otherwise specified in the environment
app.set('port', 8080);

const swaggerOptions = {
  swaggerDefinition: {
      info: {
          title: 'Blockchain Ticket API',
          description: "This is a sample server for a pet store.",
          contact: {
              name: "API Support",
              email: "colm@email.com"
          },
          servers: ["http://localhost:8080"]
      }
  },
  //Nb router ?
  apis: ["servern.js"]
};

const swaggerDocs = swaggerJsDoc(swaggerOptions);
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocs));

app.use(express.json());

//Routes Below here with swagger definitions
/**
 * @swagger
 * /tickets/{address}/{privateKey}: 
 *  get:
 *      description: Use to request all tickets at submitted account.
 *      parameters:
 *      - in: path
 *        name: address
 *        type: string
 *        required: true
 *        description: Blockchain Address.
 *      - in: path
 *        name: privateKey
 *        type: string
 *        required: true
 *        description: Key corresponding to Blockchain Address.  
 *      responses:
 *          '200':
 *              description: A succesful response 
 *  
 */
app.get('/tickets/:address/:privateKey', (req, res) =>{
  objaccount1.address = req.params.address;
  objaccount1.pKey = Buffer.from(req.params.privateKey, 'hex');   
  
  componentWillMount().then(
      res.status(200).send(objvars.colors)
)
});

/**
 * @swagger
 * /buyticket:
 *    post:
 *      description: Use to generate new ticket token
 *    parameters:
 *      - name: ticket
 *        in: body
 *        description: Object to create new ticket
 *        required: true
 *        schema:
 *          type: object
 *          required:
 *              -id
 *              -address
 *              -privateKey
 *          properties:
 *              id:
 *                  type: string
 *              address:
 *                  type: string
 *              privateKey:
 *                  type: string      
 *    responses:
 *      '200':
 *        description: Successfully sent to blockchain
 */

app.post('/buyticket', (req, res) =>{
  
  const { id } = req.body;
  objaccount1.address = req.body.address;
  objaccount1.pKey = Buffer.from(req.body.privateKey, 'hex'); 

  componentWillMount()
  setTimeout(() =>{
    mint2(id)  
      
    if (!id){
        res.status(418).send({ message: 'We need an ID '})    
    }
    res.status(200).send({
        tshirt: `Ticket has an ID of ${id}`,
    })
  }, 
  2000)
  
});

/**
 * @swagger
 * /transferticket:
 *    put:
 *      description: Use to transfer a ticket token
 *    parameters:
 *      - name: ticket
 *        in: body
 *        description: Object to update a ticket owner
 *        required: true
 *        schema:
 *          type: object
 *          required:
 *              -id
 *              -address
 *              -primaryKey
 *          properties:
 *              id:
 *                  type: string
 *              address:
 *                  type: string
 *              primaryKey:
 *                  type: string      
 *    responses:
 *      '200':
 *        description: Successfully sent to blockchain
 */



//Below here Blockchain Fucntions

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
    console.log(Color.networks[networkID])
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
    //console.log(objvars.colors)
    } else {
      //console.log('****Not Useable****')
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
  console.log("The following ticket have been issued on the blockchain:" + req.params.colorid)  
  mint2(req.params.colorid)
    componentWillMount().then(
      res.send(objvars.colors)
  )
  });  

  app.get("/transfer", function(req, res) {
    
    console.log("Transfer Executed")
      res.send()
   });    



// listen for requests :)
var listener = app.listen(8080, function () {
    console.log('Your app is listening on port ' + listener.address().port);
  });  