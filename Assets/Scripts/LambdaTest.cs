using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LambdaTest : MonoBehaviour {
    public int i = 1;
    public List<int> list = new List<int> {1, 2, 3};
    public List<Chara> charas = new List<Chara> {
        new Chara(26, "Henri"),
        new Chara(28, "Elie"),
        new Chara(87, "Pépito")
    };

    public int j => i+1;

    public void l() => i++;
    public int k() => i + 1;
    public int m(int a) => a + 1;
    public void _m(int a) => i+=a;

    private void Update() {
        this.Wait(1, () => i++);
        this.Wait(1, l);
        this.Wait(1, () => l());
        this.Wait(1, () => _m(1));

        //Where: sélectionner une sous-liste qui remplit une propriété
        //(liste de tous les éléments supérieurs à 2 de ma liste)
        List<int> list2 = list.Where((n) => n > 2).ToList();
        List<int> list3 = list.Where(n => n > 2).ToList();

        //Select: opérer une transformation sur chaque élément de la liste
        //(liste des noms de mes personnages)
        List<string> list4 = charas.Select(chara => chara.name).ToList();
        
        //Foreach: effectuer une opération pour chaque élément de la liste
        //(ajouter 1 à l'age des personnages)
        charas.ForEach(item => item.age++);
    }
}

[Serializable]
public class Chara {
    public int age;
    public string name;

    public Chara(int age, string name) {
        this.age = age;
        this.name = name;
    }
}