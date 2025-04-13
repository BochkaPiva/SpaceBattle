namespace SpaceBattle.Lib;
interface IReceiver
{
    ICommand Receive();
    bool isEmpty();
}
