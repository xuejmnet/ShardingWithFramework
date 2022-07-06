using TodoApp;

Serve.Run(RunOptions.Default
    .AddComponent<ShardingCoreComponent>()
    .UseComponent<UseShardingCoreComponent>());