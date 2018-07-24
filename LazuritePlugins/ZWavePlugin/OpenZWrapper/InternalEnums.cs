using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    internal enum ZWControllerState : byte
    {
        Normal = 0,             /**< No command in progress. */
        Starting,               /**< The command is starting. */
        Cancel,                 /**< The command was canceled. */
        Error,                  /**< Command invocation had error(s) and was aborted */
        Waiting,                /**< Controller is waiting for a user action. */
        Sleeping,               /**< Controller command is on a sleep queue wait for device. */
        InProgress,             /**< The controller is communicating with the other device to carry out the command. */
        Completed,              /**< The command has completed successfully. */
        Failed,                 /**< The command has failed. */
        NodeOK,                 /**< Used only with ControllerCommand_HasNodeFailed to indicate that the controller thinks the node is OK. */
        NodeFailed              /**< Used only with ControllerCommand_HasNodeFailed to indicate that the controller thinks the node has failed. */
    }
}