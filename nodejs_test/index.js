//expressモジュールをインポート
const express = require('express');
const bodyParser = require("body-parser");
const server = express();
const subPage = require("./subPage.js");


//初期設定
server.use(bodyParser.urlencoded({ extended: true }));
server.use(express.static("public"));
server.use("/subPage", subPage);

server.post("/POST", (req, res) => {
    if(req.body["PageChange"]) {
        res.redirect("/subPage");
    }
    if(req.body["PageReLoad"]) {
        console.log(req.body["PageChange"]);
        res.redirect("/");
    }
})

server.get('/', function (req, res) {
  res.sendFile(__dirname + "/index.html");
});
 
 
server.listen(3000);
console.log('Server running at http://localhost:3000/');

