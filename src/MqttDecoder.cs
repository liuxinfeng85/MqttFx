﻿using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using nMqtt.Packets;

namespace nMqtt
{
    public sealed class MqttDecoder : ByteToMessageDecoder
    {
        readonly bool isServer;
        readonly int maxMessageSize;

        public MqttDecoder(bool isServer, int maxMessageSize)
        {
            this.isServer = isServer;
            this.maxMessageSize = maxMessageSize;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                if (!TryDecodePacket(context, input, out Packet packet))
                    return;

                output.Add(packet);
            }
            catch (DecoderException)
            {
                input.SkipBytes(input.ReadableBytes);
                throw;
            }
        }

        bool TryDecodePacket(IChannelHandlerContext context, IByteBuffer buffer, out Packet packet)
        {
            if (!buffer.IsReadable(2))
            {
                packet = null;
                return false;
            }

            packet = MqttPacketFactory.CreatePacket(buffer);

            return true;
        }
    }
}