import React, { useState } from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';

const API = window.API;//使用するAPIは@typesで宣言し、API.api名で実行する
type stockRow = {
  ID: number,
  Stock: number,
  Item: string
}


//アプリケーション本体
export const App = () => {
  const [text, setText] = useState();
  const [stockData, setStockData] = useState<Array<stockRow>>(); 

  const date = () => {
    API.GetDate().then((val) => setText(val));
  }

  const stock = () => {
    API.GetStock().then((val) => {
      console.log(val);
      setStockData(val.recordset)
    });
  }

  return (
    <div>
      <p>{text}</p>
      <button type="button" onClick={date}>GetDate!</button>
      <button type="button" onClick={stock}>GetStock!</button>

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell>Stock</TableCell>
            <TableCell>Item</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {stockData !== undefined? stockData.map((row) => (
            <TableRow key={row.ID}>
              <TableCell >{row.ID}</TableCell>
              <TableCell>{row.Stock}</TableCell>
              <TableCell>{row.Item}</TableCell>
            </TableRow>
          )): null} 
        </TableBody>
      </Table>

    </div>
  );
};
