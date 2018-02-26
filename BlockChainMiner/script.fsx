#load "block.fs"
#load "miner.fs"

open BlockChain.Types
open BlockChain.Miner

let numbeOfBlocksToGenerate = 5

//#time "on"

// genesisBlock |> blockchain numbeOfBlocksToGenerate |> Seq.iter (printfn "%A")

let previousBlockWithHash = {
  block = {
    Block.index = 9L;
    minedBy = "Adam";
    data = "0";
    previousHash = "0000D6CBE5B197E5F0ED88F773A6BD03F1A5D6E39A7C75E297DC1D12685BE5C8";
    nonce = 6334L;
  };
  hash = "0000DEA9218EB34470BAD8C08D8BC1FD73D304739FD55E8652F17A3D3CD3876F"
}

previousBlockWithHash |> blockchain 1 |> Seq.iter (printfn "%A")

let stringReduce seperator (strings: seq<string>) = 
  strings 
  |> Seq.reduce (fun a b -> sprintf "%s%s%s" a seperator b)

let printBlock (block: BlockWithHash) = 
  seq{
    yield block.block.index |> string
    yield block.block.minedBy
    yield block.block.data
    yield block.block.previousHash
    yield block.block.nonce |> string
    yield block.hash
  } |> (stringReduce " ")

genesisBlock |> printBlock

