const express = require("express");
const app = express();
const path = require('path');

const PORT = 3000;
const IP = "" // YOUR IP HERE


app.use('/', express.static(path.join(__dirname, 'static')))

app.get("/", (req, res) => {
    res.sendFile(__dirname + "/static/web.html");
});

app.listen(PORT, IP); 