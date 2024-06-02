namespace Prototype
{
    public class Market : Singleton<Market>
    {
        public Trader Trader;
        public CashRegister CashRegister;
    }
}
