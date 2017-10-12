#if PLAYMAKER

using HutongGames.PlayMaker;
using UnityEngine;

namespace CloudOnce.PlayMaker.Actions
{
    [ActionCategory("CloudOnce")]
    [HutongGames.PlayMaker.Tooltip("Run this action anywhere you want the players to log in to the native services")]
    public class InitializeCloudOnce : FsmStateAction
    {
        
        public FsmBool CloudSave;
        public FsmBool AutoSignIn;
        public FsmBool AutoCloudLoad;

        public override void Reset()
        {
            CloudSave = true;
            AutoSignIn = true;
            AutoCloudLoad = true;
        }

        public override void OnEnter()
        {
            Cloud.Initialize(CloudSave.Value, AutoSignIn.Value, AutoCloudLoad.Value);

            Finish();
        }
    }
}

#endif