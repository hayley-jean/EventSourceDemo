using System;

namespace EventSourceDemo
{
    public class OrderAlreadyCompleted : Exception
    {
    }

    public class ItemNotInCart : Exception
    {
    }
}