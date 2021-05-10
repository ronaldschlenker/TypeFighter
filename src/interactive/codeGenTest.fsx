
#load "visuBase.fsx"
open VisuBase
open TypeFighter
open TypeFighter.DotNetCodeGen
open TestBase

//#r "nuget: Basic.Reference.Assemblies"
//#r "nuget: Microsoft.CSharp"
//#r "nuget: Microsoft.CodeAnalysis"
//#r "nuget: Microsoft.CodeAnalysis.CSharp"

let solve env exp =
    let annoRes = AnnotatedAst.create env exp
    let nodes = annoRes.resultExp |> ConstraintGraph.create
    let res = ConstraintGraph.solve annoRes.newGenVar nodes
    ConstraintGraph.applyResult annoRes.resultExp res.allNodes
let renderDisplayClasses env exp =
    //let exp = App (Abs "__" exp) (Num 0.0)
    exp
    |> solve env 
    |> fun (exp,tyvarToTaus) -> renderDisplayClasses (RecordCache()) tyvarToTaus exp
    |> List.map (fun res ->
        printfn "------------------"
        printfn "%s" res
        printfn "------------------")
let render env exp =
    exp
    |> solve env 
    |> fun (exp,tyvarToTaus) -> render exp tyvarToTaus
    |> fun res ->
        printfn "------------------"
        printfn ""
        printfn "%s" res.records
        printfn ""
        printfn "%s" res.body
        printfn ""
        printfn "------------------"




let env1 = env [ map; add; numbers ]

(*
let x = 10.0
map Numbers (number ->
    add number x)
*)

(Let "x" (Num 10.0)
(Map (Var "Numbers") (Abs "number"
    (Appn (Var "add") [ Var "number"; Var "x" ] ))))
|> renderDisplayClasses env1
//|> render env1





(*
    let id = fun x -> { whatever = 23.0 }
    { myString = id "Hello World"; myNumber = id 42.0 }
*)
let env2 = env [ ]

(Let "id" (Abs "x" (Record [ "whatever", Num 23.0 ] ))
(Record [ "myString", App (Var "id") (Str "Hello World"); "myNumber", App (Var "id") (Num 42.0) ])
)
|> renderDisplayClasses env2



(*
    let id = fun x -> x
    let add = fun a -> fun b -> { a = a; b = b }
    add "Hello" (id 42.0)
*)
let env3 = env [ ]

(Let "id" (Abs "x" (Var "x"))
(Let "add" (Abs "a" (Abs "b" (Record [ "field1", Var "a"; "field2", Var "b" ])))
(App (App (Var "add") (Str "Hello")) (App (Var "id") (Num 42.0)))
))
|> renderDisplayClasses env3
//|> showSolvedAst env3


