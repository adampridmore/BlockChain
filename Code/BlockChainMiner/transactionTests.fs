﻿module transactionTests
open Xunit
open BlockChain.Types
open BlockChain.Transaction

[<Fact>]
let ``transaction to string``() =
    let txt =
        { from = "Adam"; ``to``= "Dave"; ammount = 100 }
        |> transactionToString 
    
    Assert.Equal("Transaction,Adam,Dave,100", txt)

[<Fact>]
let ``parse transaction``() =
    let t = parseTransaction "Transaction,Adam,Dave,100"
    Assert.True(t.IsSome)
    Assert.Equal("Adam", t.Value.from)
    Assert.Equal("Dave", t.Value.``to``)
    Assert.Equal(100, t.Value.ammount)

[<Fact>]
let ``apply from transaction``()=
    let miner = {name= "Adam"; balance=300};
    let transaction = { from = "Adam"; ``to``= "Dave"; ammount = 100 }
    Assert.Equal(200, (transaction |> applyTransaction miner).balance)

[<Fact>]
let ``apply to transaction``()=
    let miner = {name= "Adam"; balance=300};
    let transaction = { from = "Dave"; ``to``= "Adam"; ammount = 100 }
    Assert.Equal(400, (transaction |> applyTransaction miner).balance)

[<Fact>]
let ``apply from and to transaction``()=
    let miner = {name= "Adam"; balance=300};
    let transaction = { from = "Adam"; ``to``= "Adam"; ammount = 100 }
    Assert.Equal(300, (transaction |> applyTransaction miner).balance)

[<Fact>]
let ``apply other transaction``()=
    let miner = {name= "Adam"; balance=300};
    let transaction = { from = "Dave"; ``to``= "Fred"; ammount = 100 }
    Assert.Equal(300, (transaction |> applyTransaction miner).balance)

[<Fact>]
let ``apply transactions to miners``()=
    let miners = [  { name = "Adam";balance = 100};
                    { name = "Dave";balance = 200}  ]
    let transaction = { from ="Adam"; ``to`` = "Dave"; ammount = 30 }

    let newMiners = applyTransactionToMiners miners transaction |> Seq.toList
    
    Assert.Equal(2, newMiners.Length)
    Assert.Equal("Adam", newMiners.[0].name)
    Assert.Equal(70, newMiners.[0].balance)
    Assert.Equal("Dave", newMiners.[1].name)
    Assert.Equal(230, newMiners.[1].balance)

[<Fact>]
let ``apply transactions to miners when missing miner``()=
    let miners = [  ]
    let transaction = { from ="Adam"; ``to`` = "Dave"; ammount = 30 }

    let newMiners = applyTransactionToMiners miners transaction |> Seq.toList
    
    Assert.Equal(2, newMiners.Length)
    Assert.Equal("Adam", newMiners.[0].name)
    Assert.Equal(-30, newMiners.[0].balance)
    Assert.Equal("Dave", newMiners.[1].name)
    Assert.Equal(30, newMiners.[1].balance)




    