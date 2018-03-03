module BlockChain.Miner

open System
open System.Security.Cryptography
open BlockChain.Types

let computeHash (b: byte[]) = b |> SHA256.Create().ComputeHash

let internal hashString (content : String) = 
    let bytes = content |> System.Text.ASCIIEncoding.UTF8.GetBytes |> computeHash
    BitConverter.ToString( bytes ).Replace("-", "")

let internal blockHash (block: Block) = 
    [block.index |> string; block.minedBy; block.data; block.previousHash; block.nonce |> string]
    |> Seq.reduce (fun a b -> sprintf "%s %s" a b)
    |> hashString

let internal isValidHash (hash:String) = hash.StartsWith("0000")

type IsValidBlock = 
  | Valid 
  | Invalid of (string seq)

let isValidBlock (block:BlockWithHash) (lastBlock:BlockWithHash) = 
  let validation =
    seq{
      yield block.block |> blockHash = block.hash, "Hash does not match hash of block." 
      yield (block.hash |> isValidHash), "Hash does not meet rules."
      yield (lastBlock.hash = block.block.previousHash), "Previous hash does not match."
      yield (lastBlock.block.index + 1) = (block.block.index), "Invalid index."
    }
    |> Seq.where(fun (valid , _) -> not valid)
    |> Seq.toList
    
  match (validation |> List.isEmpty ) with
  | true -> Valid
  | false -> Invalid(validation |> Seq.map(snd))

let newBlock minedBy data (previousBlock: BlockWithHash) = 
    let nonce, hash = 
        Seq.initInfinite id
        |> Seq.map(fun nonce -> 
            let block = {
                index = (previousBlock.block.index + 1)
                minedBy = minedBy
                data = data
                nonce = nonce
                previousHash = previousBlock.hash
            }

            let hash =  block |> blockHash
            nonce,hash)
        |> Seq.where (fun (_, hash) -> isValidHash hash)
        |> Seq.head
            
    let block = {
        index = previousBlock.block.index + 1;
        minedBy = minedBy;
        data = data;
        nonce = nonce;
        previousHash = previousBlock.hash;
    }

    {
        block = block;
        hash = hash
    }

let genesisBlock =
  { 
    block = 
      {
        index = 0;
        minedBy = "Genesis"
        data = "Genesis";
        previousHash = "0";
        nonce = 52458;
      }
    hash = "000021C1766F55BD5D413F0AC128A5D3D6B50E4F0D608B653209C4D468232C11" // block |> blockHash 
  }

let stringReduce seperator (strings: seq<string>) = 
  strings 
  |> Seq.reduce (fun a b -> sprintf "%s%s%s" a seperator b)

let sprintBlock (block: BlockWithHash) = 
  seq{
    yield block.block.index |> string
    yield block.block.minedBy
    yield block.block.data
    yield block.block.previousHash
    yield block.block.nonce |> string
    yield block.hash
  } |> (stringReduce " ")

//let numbeOfBlocksToGenerate = 5

let tuple a = (a, a)

let blockchain numbeOfBlocksToGenerate minedBy lastBlock =
  Seq.initInfinite id
  |> Seq.take numbeOfBlocksToGenerate
  |> Seq.map string
  |> Seq.mapFold (fun previousBlock data -> previousBlock |> newBlock minedBy data |> tuple ) lastBlock
  |> fst