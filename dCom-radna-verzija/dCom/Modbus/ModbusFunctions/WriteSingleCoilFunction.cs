using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters p = (ModbusWriteCommandParameters)CommandParameters;

            byte[] request = new byte[12];

            // MBAP HEADER
            request[0] = (byte)(p.TransactionId >> 8);
            request[1] = (byte)(p.TransactionId & 0xFF);

            request[2] = 0x00;
            request[3] = 0x00;

            request[4] = 0x00;
            request[5] = 0x06;

            request[6] = p.UnitId;

            // PDU
            request[7] = p.FunctionCode;

            request[8] = (byte)(p.OutputAddress >> 8);
            request[9] = (byte)(p.OutputAddress & 0xFF);

            
            if (p.Value == 1)
            {
                request[10] = 0xFF;
                request[11] = 0x00;
            }
            else
            {
                request[10] = 0x00;
                request[11] = 0x00;
            }

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> result =new Dictionary<Tuple<PointType, ushort>, ushort>();

            ushort address = (ushort)((response[8] << 8) | response[9]);

            ushort value = (response[10] == 0xFF) ? (ushort)1 : (ushort)0;

            result[new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address)] = value;

            return result;
        }
    }
}