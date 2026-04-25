using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;

            byte[] request = new byte[5];

            request[0] = (byte)(p.StartAddress >> 8);
            request[1] = (byte)(p.StartAddress & 0xFF);

            request[2] = (byte)(p.Quantity >> 8);
            request[3] = (byte)(p.Quantity & 0xFF);

            request[4] = 0x00;

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[1];

            for (int i = 0; i < byteCount / 2; i++)
            {
                ushort value = (ushort)((response[2 + i * 2] << 8) | response[3 + i * 2]);

                ushort address = (ushort)(i);

                result.Add(new Tuple<PointType, ushort>(PointType.HR_LONG, address), value);
            }

            return result;
        }
    }
}