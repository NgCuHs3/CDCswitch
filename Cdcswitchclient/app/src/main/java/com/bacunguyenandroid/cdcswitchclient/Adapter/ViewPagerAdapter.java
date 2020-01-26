package com.bacunguyenandroid.cdcswitchclient.Adapter;

import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;

import com.bacunguyenandroid.cdcswitchclient.Fragment.ChooseFra;
import com.bacunguyenandroid.cdcswitchclient.Fragment.ConnectFra;


public class ViewPagerAdapter extends FragmentPagerAdapter {
    public ViewPagerAdapter(FragmentManager fm) {
        super(fm);
    }


    private ConnectFra connectFra = null;
    private ChooseFra chooseFra = null;
    public  ConnectFra getConnectFra(){
        return  connectFra;
    }
    public ChooseFra getChooseFra(){return  chooseFra;}
    @Override
    public Fragment getItem(int i) {

        if (i == 0)
        {
            connectFra = new ConnectFra();
            return  (Fragment) connectFra;
        }
        else if (i == 1)
        {
            chooseFra = new ChooseFra();
            return (Fragment) chooseFra;
        }
        return  null;
    }

    @Override
    public int getCount() {
        return 2;
    }

    @Override
    public CharSequence getPageTitle(int position) {
        String title = null;
        if (position == 0)
        {
            title = "Connect";
        }
        else if (position == 1)
        {
            title = "Choose";
        }
        return title;
    }
}
