//引数に指定したDBに接続するためのmssqlの設定インスタンスを生成する
const CreateConfig = (databaseName: string) => {
    let config = {
        user: 'appUser',
        password: 'appUser',
        server: 'MOUSE-MORIYAMA\\SQLEXPRESS',
        database: databaseName,
        port: 1433,
        options: {
            trustServerCertificate: true
        }
    }
    
    return config;
}

//現在日時を取得する
export const GetDate = () => {
    let date = new Date();
    let outString = 
    date.getFullYear() + "年" + 
    (date.getMonth() + 1)  + "月" + 
    date.getDate() + "日" + 
    date.getHours() + "時" + 
    date.getMinutes() + "分" + 
    date.getSeconds() + "秒";

    return outString;
}

//SQLServerに接続してStoringDBからデータを持ってくる
export const GetStock = async () => {
    let sql = require('mssql');
    const exec = async () => {
        try {
            await sql.connect(CreateConfig('TEST'))
            const result = await sql.query('SELECT * FROM TEST.dbo.Storing')
            return result;
        } catch (err) {
            return err;
        }
    }

    return await exec();
}