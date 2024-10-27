using System;
using System.Collections.Generic;

namespace GildedRoseKata;

public interface IUpdateItemStrategy
{
    public Item Update(Item item);
}

public abstract class ItemStrategy : IUpdateItemStrategy
{
    public bool IsValidQuality(int quality) => quality is >= 0 and <= 50;

    public virtual void UpdateQuality(Item item)
    {
        if (IsValidQuality(item.Quality))
        {
            item.Quality = item.SellIn <= 0 ? item.Quality - 2 : item.Quality--;
        }
    }

    public virtual void UpdateSellIn(Item item)
    {
        item.SellIn = item.SellIn--;
    }

    // default behaviour:
    //  => 0 quality =< 50
    //  daily quality--, sellIn--
    //  if sellin =< 0, quality decreases 2x
    public virtual Item Update(Item item)
    {
        UpdateSellIn(item);
        UpdateQuality(item);

        return item;
    }
}
public class DefaultUpdateStrategy 
    : ItemStrategy
{
}

//  if Brie, quality++
public class InverseQualityUpdateStrategy
    : ItemStrategy
    , IUpdateItemStrategy
{
    public override void UpdateQuality(Item item)
    {
        var tempQuality = item.Quality++;

        if (IsValidQuality(tempQuality)) 
        {
            item.Quality = tempQuality;
        } 
    }
}

//  if Sulfuras, quality and sellIn are static
public class StaticUpdateStrategy
    : ItemStrategy
    , IUpdateItemStrategy
{
    public override void UpdateQuality(Item item) { return; }
    public override void UpdateSellIn(Item item) { return; }
}

//  if Conjured, quality 2x (default sellIn = 0 behaviour)
public class ConjouredUpdateStrategy
    : ItemStrategy
    , IUpdateItemStrategy
{
    //  if Conjured, quality 2x (default sellIn = 0 behaviour),
    //  but what happens when it sellIn = 0?
    public override void UpdateQuality(Item item)
    {
        item.Quality =- 2;
    }
}

//  if Conjured, quality 2x (default sellIn = 0 behaviour)
public class BackstagePassUpdateStrategy
    : ItemStrategy
    , IUpdateItemStrategy
{
    //  if Backstage,
    //  quality +2 if 5> sellIn =<10,
    //  quality +3 if 0> sellIn =<5,
    //  quality =0 if sellIn <= 0, 
    public override void UpdateQuality(Item item)
    {
        if (!IsValidQuality(item.Quality))
        {
            return;
        }

        if (item.SellIn <= 0)
        {
            item.Quality = 0;
        }
        else if (item.SellIn is > 5 and <= 10)
        {
            item.Quality += 2;
        }
        else if (item.SellIn is > 0 and <= 5) 
        {
            item.Quality += 3;
        }
        else
        {
            base.UpdateQuality(item);
        }
    }
}

public class GildedRose
{
    IList<Item> Items;

    public GildedRose(IList<Item> Items)
    {
        this.Items = Items;
    }

    // strategy pattern -> factory pattern -> decorator?
    // factory builds strategy based on name or characteristics?

    // behaviour:
    //  => 0 quality =< 50
    //  daily quality--, sellIn--
    //  if sellin =< 0, quality decreases 2x
    //  if Brie, quality++
    //  if Sulfuras, quality and sellIn are static
    //  if Backstage, quality + 2 if 5> sellIn =<10, quality +3 if 0> sellIn =<5, if sellIn <= 0, quality = 0
    //  if Conjured, quality 2x (default sellIn = 0 behaviour)
    public void UpdateQuality()
    {
        foreach (var item in Items) 
        {
            IUpdateItemStrategy strategy;

            // exact match or is contains good enough?
            if (item.Name.Contains("Aged Brie"))
            {
                strategy = new InverseQualityUpdateStrategy();
            }
            else if (item.Name.Contains("Sulfuras"))
            {
                strategy = new StaticUpdateStrategy();
            }
            else if (item.Name.Contains("Backstage"))
            {
                strategy = new BackstagePassUpdateStrategy();
            }
            else if (item.Name.Contains("Conjured"))
            {
                strategy = new ConjouredUpdateStrategy();
            }
            else
            {
                // default
                strategy = new DefaultUpdateStrategy();
            }

            var newItem = strategy.Update(item);

        }
        /* old code
       for (var i = 0; i < Items.Count; i++)
       {
           if (Items[i].Name != "Aged Brie" && Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
           {
               if (Items[i].Quality > 0)
               {
                   if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                   {
                       Items[i].Quality = Items[i].Quality - 1;
                   }
               }
           }
           else
           {
               if (Items[i].Quality < 50)
               {
                   Items[i].Quality = Items[i].Quality + 1;

                   if (Items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
                   {
                       if (Items[i].SellIn < 11)
                       {
                           if (Items[i].Quality < 50)
                           {
                               Items[i].Quality = Items[i].Quality + 1;
                           }
                       }

                       if (Items[i].SellIn < 6)
                       {
                           if (Items[i].Quality < 50)
                           {
                               Items[i].Quality = Items[i].Quality + 1;
                           }
                       }
                   }
               }
           }

           if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
           {
               Items[i].SellIn = Items[i].SellIn - 1;
           }

           if (Items[i].SellIn < 0)
           {
               if (Items[i].Name != "Aged Brie")
               {
                   if (Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
                   {
                       if (Items[i].Quality > 0)
                       {
                           if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
                           {
                               Items[i].Quality = Items[i].Quality - 1;
                           }
                       }
                   }
                   else
                   {
                       Items[i].Quality = Items[i].Quality - Items[i].Quality;
                   }
               }
               else
               {
                   if (Items[i].Quality < 50)
                   {
                       Items[i].Quality = Items[i].Quality + 1;
                   }
               }
           }
       }
   */
    }
}