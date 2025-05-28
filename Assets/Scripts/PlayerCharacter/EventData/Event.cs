using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Event{
    int id;
    string name;
    Type type;
    string description;

    public Event(int id){
        this.id = id;
        
        Dictionary<string,object> eventData = DatabaseManager.Instance.GetSingleRowFromQuery(
            $"SELECT * FROM event WHERE id = {id};"
        );
        
        name = eventData["name"].ToString();
        string preType = eventData["type"].ToString();
        switch (preType){
            case "boss":
                type = Type.BOSS;
                break;
            case "achievement":
                type = Type.ACHIEVEMENT;
                break;
            case "event":
                type = Type.EVENT;
                break;
            default:
                type = Type.EVENT;
                break;
        }
        description = eventData["description"].ToString();
    }

    public Event(string eventName){
        this.name = eventName;
        
        Dictionary<string,object> eventData = DatabaseManager.Instance.GetSingleRowFromQuery(
            $"SELECT * FROM event WHERE name = '{eventName}';"
        );

        if (eventData == null) return;

        id = Convert.ToInt32(eventData["id"]);
        string preType = eventData["type"].ToString();
        switch (preType){
            case "boss":
                type = Type.BOSS;
                break;
            case "achievement":
                type = Type.ACHIEVEMENT;
                break;
            case "event":
                type = Type.EVENT;
                break;
            default:
                type = Type.EVENT;
                break;
        }
        description = eventData["description"].ToString();

        this.ToString();
    }

    public override string ToString(){
        return $"(EVENT {id}) {name}: {description} ({type})";
    }

    public int GetId() => id;
    public string GetName() => name;
    public string GetDescription() => description;
}
