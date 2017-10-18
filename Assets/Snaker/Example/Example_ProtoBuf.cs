using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using SGF;
using UnityEngine;

public class Example_ProtoBuf : MonoBehaviour {

    [ProtoContract]
    public class UserInfo
    {
        [ProtoMember(1)]
        public int id;
        [ProtoMember(2)]
        public string name;
        [ProtoMember(3)]
        public string title;
    }


	// Use this for initialization
	void Start () 
    {
        UserInfo info1 = new UserInfo();
	    info1.id = 123;
	    info1.name = "slicol";
	    info1.title = "student";

	    byte[] buff = PBSerializer.NSerialize(info1);

	    var info2 = PBSerializer.NDeserialize<UserInfo>(buff);
        this.LogWarning("info1 --- id:{0}, name:{1}, title:{2}", info1.id, info1.name, info1.title);
        this.LogWarning("info2 --- id:{0}, name:{1}, title:{2}", info2.id, info2.name, info2.title);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

