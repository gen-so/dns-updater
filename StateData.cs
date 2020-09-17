namespace dns_updater
{
    public class StateData
    {
        private bool _previousSendPassed;


        public StateData()
        {
            //set default as not passed (when first run, it has not sent data before)
            _previousSendPassed = false;
        }

        public void SetPreviousSendFailed()
        {
            _previousSendPassed = false;
        }

        public void SetPreviousSendPassed()
        {
            _previousSendPassed = true;
        }

        public bool IsPreviousSendPassed()
        {
            return _previousSendPassed;
        }
    }
}