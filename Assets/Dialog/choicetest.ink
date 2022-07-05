INCLUDE globals.ink

{ nome_cor == "" : -> main | -> already_chose}


=== main ===
Qual cor vc escolhe?
    + [Vermelho]
        -> chosen("Vermelho")
    + [Azul]
        -> chosen("Azul")
    + [Verde]
        -> chosen("Verde")
        
=== chosen(cor) ===
//~ pokemon_name = pokemon
// You chose {pokemon}!
~ nome_cor = cor 
Você escolheu {cor}

-> END

=== already_chose ===
Vc já escolheu {nome_cor}!
-> END