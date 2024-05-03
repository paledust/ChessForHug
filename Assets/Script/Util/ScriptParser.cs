using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptParser
{
    private const string LF = "<return>";
    private const string CR = "\r";
    readonly static string[] separators = new string[]{"=="};
    public static string ParseEnvironmentScript(TextAsset textFile){return ParseEnvironmentScript(textFile.text);}
    public static string ParseEnvironmentScript(string data){
        string[] lines = data.Split('\n');
        string result = lines[Random.Range(0, lines.Length)];
        result = result.Replace(LF,"\n");
        result = result.Replace(CR, string.Empty);

        return result;
    }

    public static string ParseMomentScript(TextAsset textFile){return ParseMomentScript(textFile.text);}
    public static string ParseMomentScript(string data){
        string[] lines = data.Split('\n');
        string result = lines[Random.Range(0, lines.Length)];
        result = result.Replace(LF,"\n");
        result = result.Replace(CR, string.Empty);
        
        return result;
    }

    public static HugData ParseGenerationScript(TextAsset textFile){return ParseGenerationScript(textFile.text);}
    public static HugData ParseGenerationScript(string data){
        string[] lines = data.Split('\n');
        string result = lines[Random.Range(0, lines.Length)];
        result = result.Replace(LF,"\n");
        result = result.Replace(CR, string.Empty);

        return new HugData(){rel=CONTEXT_RELATION.FRIEND,script=result};
    }
}

// public static class DialogueParser
// {
//     const string LF = "<return>";
//     const string CR = "\r";
//     readonly static string[] separators = new string[]{"==",":"};
//     public static Subtitle[] ParseSubtitleFromTextFile(TextAsset textFile){
//         string[] lines = textFile.text.Split('\n');
//         Subtitle[] subtitles = new Subtitle[lines.Length];

//         for(int i=0; i<lines.Length; i++){
//             subtitles[i] = new Subtitle();
//         //Read the content, but replace the <return> with \n return
//             subtitles[i].content  = ParseIntersection(lines[i], out subtitles[i].intersection);
//             subtitles[i].content  = subtitles[i].content.Replace(CR,string.Empty);
//             if(subtitles[i].content.Length == 0 || subtitles[i].content == "") subtitles[i].content = string.Empty;
//         }

//         return subtitles;
//     }
//     public static Dialogue[] ParseDialogueFromTextFile(TextAsset textFile){
//         string[] lines = textFile.text.Split('\n');
//         Dialogue[] dialogues = new Dialogue[lines.Length];

//         for(int i=0; i<lines.Length; i++){
//             dialogues[i] = new Dialogue();
//             string content = ParseDialogueSpeakerIndex(lines[i], out dialogues[i].Index);
//             dialogues[i].content = ParseIntersection(content, out dialogues[i].intersection);
//         //Read the content, but replace the <return> with \n return
//             dialogues[i].content = dialogues[i].content.Replace(LF,"\n");
//             dialogues[i].content = dialogues[i].content.Replace(CR,string.Empty);
//             if(dialogues[i].content.Length == 0 || dialogues[i].content == "") dialogues[i].content = string.Empty;
//         }
//         return dialogues;
//     }
//     static string ParseIntersection(string text, out float intersection){
//         string[] keyAndValue = text.Split(separators[0], System.StringSplitOptions.None);
//         intersection = 1f;
//         if(keyAndValue.Length>1){
//             float.TryParse(keyAndValue[1], out intersection);
//         }
//         return keyAndValue[0];
//     }
//     static string ParseDialogueSpeakerIndex(string text, out int index){
//         string[] keyAndValue = text.Split(separators[1], System.StringSplitOptions.None);
//         index = 0;
//         if(keyAndValue.Length>1){
//             int.TryParse(keyAndValue[0], out index);
//             return keyAndValue[1];
//         }
//         else{
//             return keyAndValue[0];
//         }
//     }
// }
