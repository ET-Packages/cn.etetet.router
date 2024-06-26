using System;
using System.Collections.Generic;
using System.Net;

namespace ET.Server
{
    [HttpHandler(SceneType.RouterManager, "/get_router")]
    public class HttpGetRouterHandler : IHttpHandler
    {
        public async ETTask Handle(Scene scene, HttpListenerContext context)
        {
            HttpGetRouterResponse response = HttpGetRouterResponse.Create();
            const int RealmZone = 2; // Realm是所有区共用的，所以独立放到单独的一个区，看StartSceneConfig配置
            List<StartSceneConfig> realms = StartSceneConfigCategory.Instance.GetBySceneType(RealmZone, SceneType.Realm);
            foreach (StartSceneConfig startSceneConfig in realms)
            {
                // 这里是要用InnerIP，因为云服务器上realm绑定不了OuterIP的,所以realm的内网外网的socket都是监听内网地址
                response.Realms.Add(startSceneConfig.InnerIPPort.ToString());
            }
            foreach (StartSceneConfig startSceneConfig in RouterConfigSingleton.Instance.GetRouters())
            {
                response.Routers.Add($"{startSceneConfig.StartProcessConfig.OuterIP}:{startSceneConfig.Port}");
            }
            HttpHelper.Response(context, response);
            await ETTask.CompletedTask;
        }
    }
}