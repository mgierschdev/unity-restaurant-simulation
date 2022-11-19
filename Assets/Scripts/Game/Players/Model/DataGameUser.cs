using System;
using System.Collections.Generic;

public class DataGameUser
{
    public string NAME { get; set; }
    public Double GAME_MONEY { get; set; }
    public Double GEMS { get; set; }
    public Double EXPERIENCE { get; set; }
    public int LEVEL { get; set; }
    public int GRID_SIZE { get; set; }
    public string LANGUAGE_CODE { get; set; }
    public string INTERNAL_ID { get; set; }
    public string FIREBASE_AUTH_ID { get; set; }
    public string EMAIL { get; set; }
    public int AUTH_TYPE { get; set; }
    public DateTime LAST_LOGIN { get; set; }
    public DateTime CREATED_AT { get; set; }
    public List<DataGameObject> OBJECTS { get; set; }
}