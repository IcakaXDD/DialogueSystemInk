INCLUDE Global.ink

{bliga_name == "": -> main | ->already_chose}

===main===
Which bliga do you choose?

 +[Ico]
    -> chosen("Ico")
 +[Vesko]
    -> chosen("Vesko")
 +[Misho]
    -> chosen("Misho")
    
    
===chosen(bliga)===
~ bliga_name = bliga
You chose {bliga}!
->END

===already_chose===
You already chose {bliga_name}!
->END
