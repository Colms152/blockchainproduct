//Working swwagger 2.0 
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
 *              -logo
 *          properties:
 *              id:
 *                  type: string
 *              logo:
 *                  type: string    
 *    responses:
 *      '200':
 *        description: Successfully sent to blockchain
 */

app.post('/buyticket', (req, res) =>{
    //vars
    //primary key
    //address
    console.log(req.body);
    const { id } = req.body;
    const { logo } = req.body;

    if (!logo){
        res.status(418).send({ message: 'We need a logo!'})    
    }

    res.status(200).send({
        tshirt: `Tshirt ${logo} has ID of ${id}`,
    })
});


app.listen(
    PORT,
    () => console.log(`it's live on http://localhost:${PORT}`)
)

