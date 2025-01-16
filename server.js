const express = require("express");
const app = express();
const path = require('path');

const IP = "127.0.0.1"; // YOUR IP HERE
const PORT = 3000;

app.use("/", express.static(path.join(__dirname, "/static")));

app.get("/", (req, res) => {
    res.sendFile(__dirname + "/static/web.html")
});

app.listen(PORT, IP); 