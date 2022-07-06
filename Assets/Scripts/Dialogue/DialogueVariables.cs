using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.IO;

public class DialogueVariables 
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

 //   public DialogueVariables (string globalsFilepath)
    public DialogueVariables (TextAsset loadGlobalsJSON)

    {
        //compila Story
       // string inkFileContents = File.ReadAllText(globalsFilepath);
     //   Ink.Compiler compiler = new Ink.Compiler(inkFileContents);
       // Story globalsVariableStory = compiler.Compile();
        Story globalsVariableStory = new Story(loadGlobalsJSON.text);

        // incializa Story

        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach(string name in globalsVariableStory.variablesState)

        {
            Ink.Runtime.Object value = globalsVariableStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);

            Debug.Log("inicia global dialogue variables" + name + "=" + value);
        }
    }
    public void StartListening(Story story)
    {
        VariableToStory(story);
        story.variablesState.variableChangedEvent += VariableChange;
    }
    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChange;

    }
    private void VariableChange(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariableToStory(Story story)
    {
        foreach(KeyValuePair<string,Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);

        }
    }
    
}
