using SGF;
using Snaker;
using Snaker.Game;
using UnityEngine;
using SGF.UI.Framework;
using SGF.Module.Framework;
using Snaker.Service.User;
using SGF.Network;

public class AppMain : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	    //Debuger.EnableLog = true;
		//Debuger.EnableSave = true;
		Debuger.Log (Debuger.LogFileDir);

        AppConfig.Init();

	    InitServices();
	    InitBusiness();

        ModuleManager.Instance.ShowModule(ModuleDef.LoginModule);
    }

    private void InitServices()
    {
        ModuleManager.Instance.Init("Snaker.Module");

		NetworkManager.Instance.Init ();

        UIManager.Instance.Init("ui/");
        UIManager.MainPage = UIDef.UIHomePage;
        UIManager.MainScene = "Main";

        UserManager.Instance.Init();

        GameManager.Instance.Init();
    }


    private void InitBusiness()
    {
        ModuleManager.Instance.CreateModule(ModuleDef.LoginModule);
        ModuleManager.Instance.CreateModule(ModuleDef.HomeModule);
        ModuleManager.Instance.CreateModule(ModuleDef.PVEModule);
        ModuleManager.Instance.CreateModule(ModuleDef.PVPModule);
        ModuleManager.Instance.CreateModule(ModuleDef.HostModule);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

}
