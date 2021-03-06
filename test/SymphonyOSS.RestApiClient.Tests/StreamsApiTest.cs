﻿// Licensed to the Symphony Software Foundation (SSF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SSF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

namespace SymphonyOSS.RestApiClient.Tests
{
    using System;
    using System.Collections.Generic;
    using Api;
    using Api.PodApi;
    using Authentication;
    using Entities;
    using Generated.OpenApi.PodApi.Client;
    using Generated.OpenApi.PodApi.Model;
    using Moq;
    using Xunit;
    using Stream = Entities.Stream;
    using StreamType = Entities.StreamType;

    public class StreamsApiTest
    {
        private readonly StreamsApi _streamsApi;

        private readonly Mock<IApiExecutor> _apiExecutorMock;

        public StreamsApiTest()
        {
            var sessionManagerMock = new Mock<IAuthTokens>();
            sessionManagerMock.Setup(obj => obj.SessionToken).Returns("sessionToken");
            var configuration = new Configuration();
            _apiExecutorMock = new Mock<IApiExecutor>();
            _streamsApi = new StreamsApi(sessionManagerMock.Object, configuration, _apiExecutorMock.Object);
        }

        [Fact]
        public void EnsureCreateStream_uses_retry_strategy()
        {
            var uidList = new List<long>();
            _apiExecutorMock.Setup(
                apiExecutor =>
                    apiExecutor.Execute(It.IsAny<Func<UserIdList, string, Generated.OpenApi.PodApi.Model.Stream>>(), It.IsAny<UserIdList>(),
                        "sessionToken")).Returns(new Generated.OpenApi.PodApi.Model.Stream("sid"));
            _streamsApi.CreateStream(uidList);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<UserIdList, string, Generated.OpenApi.PodApi.Model.Stream>>(), It.IsAny<UserIdList>(), "sessionToken"));
        }

        [Fact]
        public void EnsureGetStreamInfo_uses_retry_strategy()
        {
            var sid = "streamId";
            _apiExecutorMock.Setup(
                apiExecutor => apiExecutor.Execute(It.IsAny<Func<string, string, StreamAttributes>>(), sid, "sessionToken")).Returns(
                new StreamAttributes("id", true, true, new Generated.OpenApi.PodApi.Model.StreamType(Generated.OpenApi.PodApi.Model.StreamType.TypeEnum.IM)));
            _streamsApi.GetStreamInfo(sid);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<string, string, StreamAttributes>>(), sid, "sessionToken"));
        }

        [Fact]
        public void EnsureCreateRoom_uses_retry_strategy()
        {
            var room = new Room("id", DateTime.UtcNow, 12345, true, "name", null, "description", true, true, true, true, true);
            _apiExecutorMock.Setup(
                apiExecutor => apiExecutor.Execute(It.IsAny<Func<V2RoomAttributes, string, V2RoomDetail>>(), It.IsAny<V2RoomAttributes>(), "sessionToken")).Returns(
                new V2RoomDetail(new V2RoomAttributes("name", null, "description", true, true, true, true, true), new RoomSystemInfo("id", 0, 12345, true)));
            _streamsApi.CreateRoom(room);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<V2RoomAttributes, string, V2RoomDetail>>(), It.IsAny<V2RoomAttributes>(), "sessionToken"));
        }

        [Fact]
        public void EnsureGetRoomInfo_uses_retry_strategy()
        {
            const string id = "id";
            _apiExecutorMock.Setup(
                apiExecutor => apiExecutor.Execute(It.IsAny<Func<string, string, V2RoomDetail>>(), id, "sessionToken")).Returns(
                new V2RoomDetail(
                    new V2RoomAttributes("name", null, "description", true, true, true, true, true),
                    new RoomSystemInfo("id", 0, 0, true)));
            _streamsApi.GetRoomInfo(id);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<string, string, V2RoomDetail>>(), id, "sessionToken"));
        }

        [Fact]
        public void EnsureSetRoomActive_uses_retry_strategy()
        {
            const string id = "id";
            const bool active = true;
            _streamsApi.SetRoomActive(id, active);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<string, bool?, string, RoomDetail>>(), id, active, "sessionToken"));
        }

        [Fact]
        public void EnsureUpdateRoom_uses_retry_strategy()
        {
            const string id = "id";
            var room = new Room("id", DateTime.UtcNow, 12345, true, "name", null, "description", true, true, true, true, true);
            _apiExecutorMock.Setup(
                apiExecutor => apiExecutor.Execute(It.IsAny<Func<string, V2RoomAttributes, string, V2RoomDetail>>(), id, It.IsAny<V2RoomAttributes>(), "sessionToken")).Returns(
                new V2RoomDetail(
                    new V2RoomAttributes("name", null, "description", true, true, true, true, true),
                    new RoomSystemInfo("id", 0, 0, true)));
            _streamsApi.UpdateRoom(room);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<string, V2RoomAttributes, string, V2RoomDetail>>(), id, It.IsAny<V2RoomAttributes>(), "sessionToken"));
        }

        [Fact]
        public void EnsureSearchRoom_uses_retry_strategy()
        {
            var searchCriteria = new RoomSearchCriteria(Query: "some_room");
            int? skip = 0;
            int? limit = 1;
            _streamsApi.SearchRoom(searchCriteria, skip, limit);
            _apiExecutorMock.Verify(obj => obj.Execute(It.IsAny<Func<string, RoomSearchCriteria, int?, int?, RoomSearchResults>>(), "sessionToken", searchCriteria, skip, limit));
        }

    }
}
