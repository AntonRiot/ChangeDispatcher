using UnityEngine;
using System.Collections;
using ru.antonriot.change_dispathcher;
using System;

/**
 * пример использования ChangeDispatcher - скелет менеджера окон.
 **/


public class SomeComponent : MonoBehaviour
{
    void Start()
    {
        ExmWindowsManager.instance.dispatcher.addChangeListener(ExmWindowChangeType.WINDOW_OPENED, onSomeWindowOpen);
        ExmWindowsManager.instance.dispatcher.addChangeListener(ExmWindowChangeType.WINDOW_CLOSED, onSomeWindowClose);

        ChangeDispatcher<ExmWindowChange> myDispatcher = new ChangeDispatcher<ExmWindowChange>();
        myDispatcher.addRedispatch(ExmWindowsManager.instance.dispatcher, ExmWindowChangeType.WINDOW_OPENED); //myDispatcher будет редиспатчить все пойманные события  WINDOW_OPENED
    }

    private void onSomeWindowOpen(ExmWindowChange change)
    {
        //узнали, что открылось окно change.target
    }

    private void onSomeWindowClose(ExmWindowChange change)
    {
        //узнали, что закрылось окно change.target
    }
}


public class ExmWindowsManager
{
    public static ExmWindowsManager instance;

    public ExmWindowsManager()
    {
        if(instance != null)
        {
            Debug.LogError("ExmWindowsManager must be singleton!");
        } else
        {
            instance = this;
        }
    }


    public ChangeDispatcher<ExmWindowChange> dispatcher = new ChangeDispatcher<ExmWindowChange>();

    public void openWindow(string title, string message)
    {
        ExmWindow w = createWindow();
        w.setData(title, message, onCloseWindow);
        
        //...

        ExmWindowChange change = ChangeDispatcher<ExmWindowChange>.GetChange();
        change.target = w;
        dispatcher.dispatchChange(change, ExmWindowChangeType.WINDOW_OPENED);

    }

    private void onCloseWindow(ExmWindow target)
    {
        ExmWindowChange change = ChangeDispatcher<ExmWindowChange>.GetChange();
        change.target = target;
        dispatcher.dispatchChange(change, ExmWindowChangeType.WINDOW_CLOSED);

        //...

    }

    private ExmWindow createWindow()
    {
        //в примере не важно что за окно и каким образом создаётся
        return new ExmWindow();
    }
}



public class ExmWindow
{
    public ChangeDispatcher<ExmWindowChange> dispatcher = new ChangeDispatcher<ExmWindowChange>();

    private Action<ExmWindow> closeCallback;

    public void setData(string _title, string _message, Action<ExmWindow> _closeCallback)
    {
        closeCallback = _closeCallback;
        //...
    }

    private void onCloseClick()
    {
        if(closeCallback != null)
        {
            closeCallback(this);
        }
    }
}


public class ExmWindowChange : ChangeObject
{
    public ExmWindow target;


    override protected void executeClear()
    {
        target = null;
    }
}


public class ExmWindowChangeType : ChangeType
{
    public static ExmWindowChangeType WINDOW_OPENED = new ExmWindowChangeType("WINDOW_OPENED");
    public static ExmWindowChangeType WINDOW_CLOSED = new ExmWindowChangeType("WINDOW_CLOSED");



    public ExmWindowChangeType(string val) : base(val)
    {
    }
}