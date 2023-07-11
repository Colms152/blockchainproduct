const express = require('express');
const app = express();
const swaggerJsDoc = require('swagger-jsdoc');
const swaggerUi = require('swagger-ui-express');



const PORT = 8080;

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
    apis: ["server_.js"]
};

const swaggerDocs = swaggerJsDoc(swaggerOptions);
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocs));

app.use(express.json())



//Routes
/**
 * @swagger
 * /tickets: 
 *  get:
 *      description: Use to request all tickets at submitted account.
 *      responses:
 *          '200':
 *              description: A succesful response 
 *  
 */
app.get('/tickets', (req, res) =>{
    res.status(200).send({
        tshirt: 'Firebase',
        size: 'large'
    })
});


var objaccount1 = {
    address: '',
    pKey: ''
}  

app.post('/buyticket', (req, res) =>{
    
    const { id } = req.body;
    const { logo } = req.body;

    objaccount1.address = req.body.address;
    console.log("This is what the address obj sees:" + req.body.address)
    objaccount1.pKey = Buffer.from(req.body.primaryKey, 'hex'); 
    console.log("This is what the pk obj sees:" + req.body.primaryKey)
    console.log(objaccount1)
  
    if (!logo){
        res.status(418).send({ message: 'We need a logo!'})    
    }
  
    res.status(200).send({
        tshirt: `Tshirt ${logo} `,
    })
    
  });

app.listen(
    PORT,
    () => console.log(`it's live on http://localhost:${PORT}`)
)

