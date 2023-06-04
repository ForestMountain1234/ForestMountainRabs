import React, { useEffect, useState } from 'react';

const API = window.API;//使用するAPIは@typesで宣言し、API.api名で実行する


//アプリケーション本体
export const App = () => {
  console.log("レンダリング")

  let [count, setCount] = useState(0);

  return (
    <div>
      {count}
      <button type="button" onClick={() => setCount(count + 1)}>click</button>
      <Test/>
    </div>
  );
};

const Test = () => {
  let [count, setCount] = useState(0);
  let [count2, setCount2] = useState(0);
  
  useEffect(() => { console.log("useEffect発火")},[count2])

  return (
    <div>
      {count}
      <button type="button" onClick = {() => setCount(count + 1)}>click</button>
      {count2}
      <button type="button" onClick = {() => setCount2(count2 + 1)}>click</button>
    </div>
  );
}