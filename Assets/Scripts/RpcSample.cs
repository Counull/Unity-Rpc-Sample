using System;
using System.Collections;
using UnityEngine;
using Best.HTTP.Shared;
using GRPC.NET;
using Grpc.Net.Client;
using Helloworld;

public class RpcSample : MonoBehaviour {
    [SerializeField] private string Url;
    [SerializeField] private ushort Prot = 50051;
    private GrpcChannel _channel;

    Greeter.GreeterClient _client;

    private void Start() {
        HTTPManager.Setup();

        //  HTTPManager.Logger.Level = Loglevels.All; // Enable all log levels
        var address = $"{Url}:{Prot}";
        _channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions {
            HttpHandler = new GRPCBestHttpHandler()
        });
        _client = new Greeter.GreeterClient(_channel);
    }


    public void SendHello() {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest() {
        Debug.Log("Say Hello");
        var reply = _client.SayHelloAsync(new HelloRequest() {Name = "Unity"});
        yield return new WaitUntil(() => reply.ResponseAsync.IsCompleted);

        if (reply.ResponseAsync.IsFaulted) {
            Debug.LogError($"请求失败: {reply.ResponseAsync.Exception}");
        }
        else {
            Debug.Log($"Say hello reply: {reply.ResponseAsync.Result.Message}");
        }
    }
}