﻿/****************************************************************************
 * 
  ____    _    _____    _      ____             _        _   
 / ___|  / \  | ____|  / \    / ___|  ___   ___| | _____| |_ 
 \___ \ / _ \ |  _|   / _ \   \___ \ / _ \ / __| |/ / _ \ __|
  ___) / ___ \| |___ / ___ \   ___) | (_) | (__|   <  __/ |_ 
 |____/_/   \_\_____/_/   \_\ |____/ \___/ \___|_|\_\___|\__|
                                                             

*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：yswenli
*命名空间：SAEA.Sockets.Core
*文件名： ArgsPool
*版本号： v4.1.2.5
*唯一标识：e22e6de2-7921-45b8-a0ad-254bea7e8bdf
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/3/9 17:56:47
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/3/9 17:56:47
*修改人： yswenli
*版本号： v4.1.2.5
*描述：
*
*****************************************************************************/
using SAEA.Sockets.Interface;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace SAEA.Sockets.Core
{
    public class ArgsPool
    {
        ConcurrentQueue<SocketAsyncEventArgs> _pool = new ConcurrentQueue<SocketAsyncEventArgs>();

        EventHandler<SocketAsyncEventArgs> _completed = null;

        bool _cache = false;

        public ArgsPool(EventHandler<SocketAsyncEventArgs> completed, bool cache = false, int size = 100000)
        {
            _completed = completed;

            _cache = cache;

            if (_cache)
            {
                for (int i = 0; i < size * 2; i++)
                {
                    var args = new SocketAsyncEventArgs();
                    args.Completed += _completed;
                    _pool.Enqueue(args);
                }
            }
        }

        public SocketAsyncEventArgs GetArgs(IUserToken userToken, bool seted = false)
        {
            if (!_pool.TryDequeue(out SocketAsyncEventArgs args))
            {
                args = new SocketAsyncEventArgs();
                args.Completed += _completed;
            }
            args.UserToken = userToken;
            if (seted)
            {
                args.SetBuffer(userToken.Buffer, 0, userToken.Buffer.Length);
            }
            return args;
        }

        public void Free(SocketAsyncEventArgs args)
        {
            if (args != null)
            {
                if (_cache)
                {
                    args.AcceptSocket = null;
                    args.UserToken = null;
                    _pool.Enqueue(args);
                }
                else
                {
                    args.Dispose();
                    args = null;
                }
            }
        }

        public void Clear()
        {
            //if (_pool != null)
                //_pool.Clear();
        }


    }
}
