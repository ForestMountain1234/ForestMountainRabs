//expressモジュールをインポート
var express = require('express');
const router =  express.Router();

//ルーティング設定
router.get('/', function (req, res) {
    console.log('hello world!!');
    res.send("こんにちは　世界");
});
 


module.exports = router;