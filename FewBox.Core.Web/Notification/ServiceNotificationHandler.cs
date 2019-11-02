﻿using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Notification
{
    public class ServiceNotificationHandler : INotificationHandler
    {
        private NotificationConfig NotificationConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }
        public ServiceNotificationHandler(NotificationConfig notificationConfig, ITryCatchService tryCatchService)
        {
            this.NotificationConfig = notificationConfig;
            this.TryCatchService = tryCatchService;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                this.TryCatchService.TryCatchWithoutNotification(() =>
                {
                    RestfulUtility.Post<NotificationRequestDto, NotificationResponseDto>($"{this.NotificationConfig.Protocol}://{this.NotificationConfig.Host}:{this.NotificationConfig.Port}/api/notification", new Package<NotificationRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new NotificationRequestDto
                        {
                            Name = name,
                            Param = param
                        }
                    });
                });
            });
        }
    }
}