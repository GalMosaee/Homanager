using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.MachineLearning.Rules;

namespace Homanager.Models
{
    public class RecommendedProduct
    {
        private Dictionary<int, string> allProducts;
        private SortedSet<int>[] dataset;

        // constractor
        public RecommendedProduct(List<Dictionary<int, string>> carts)
        {
            // Let the database of transactions consist of following itemsets:
            allProducts = new Dictionary<int, string>();
            if (carts == null || carts.Count == 0)
                return;
            dataset = new SortedSet<int>[carts.Count];
            for(int i = 0; i < carts.Count; i++)
            {
                var cartProducts = new SortedSet<int>();
                foreach (var product in carts[i])
                {
                    var key = product.Key;
                    if (!allProducts.Values.Contains(product.Value))
                        allProducts.Add(product.Key, product.Value);
                    else
                        key = allProducts.FirstOrDefault(x => x.Value == product.Value).Key;
                    cartProducts.Add(key);
                }
                dataset[i] = cartProducts;
            }
        }

        public string RecommendOnProduct(Dictionary<int,string> dictCurrentCart)
        {
            if (dictCurrentCart.Count == 0)
                return string.Empty;

            // We will use Apriori to determine the frequent item sets of this database.
            // To do this, we will say that an item set is frequent if it appears in at 
            // least 3 transactions of the database: the value 3 is the support threshold.

            // Create a new a-priori learning algorithm with support 3
            Apriori apriori = new Apriori(threshold: 2, confidence: 0);

            // Use the algorithm to learn a set matcher
            AssociationRuleMatcher<int> classifier = apriori.Learn(dataset);

            // Use the classifier to find orders that are similar to 
            // orders where clients have bought items 1 and 2 together:
            var products = new int[dictCurrentCart.Count];
            int index = 0;
            foreach (var product in dictCurrentCart)
            {
                products[index] = product.Key;
                index++;
            }
            int[][] matches = classifier.Decide(products);

            if (matches.Length == 0)
                return string.Empty;
            else if (!allProducts.Keys.Contains(matches[0][0])) // the first product recommended
                return string.Empty;

            AssociationRule<int>[] rules = classifier.Rules;
            string recProduct;
            bool res = allProducts.TryGetValue(matches[0][0], out recProduct);
            if (res)
                return recProduct;
            return string.Empty;
            // The result should be:
            // 
            //   new int[][]
            //   {
            //       new int[] { 4 },
            //       new int[] { 3 }
            //   };

            // Meaning the most likely product to go alongside the products
            // being bought is item 4, and the second most likely is item 3.

            // We can also obtain the association rules from frequent itemsets:

            // The result will be:
            // {
            //     [1] -> [2]; support: 3, confidence: 1, 
            //     [2] -> [1]; support: 3, confidence: 0.5, 
            //     [2] -> [3]; support: 3, confidence: 0.5, 
            //     [3] -> [2]; support: 3, confidence: 0.75, 
            //     [2] -> [4]; support: 4, confidence: 0.66, 
            //     [4] -> [2]; support: 4, confidence: 0.8, 
            //     [3] -> [4]; support: 3, confidence: 0.75, 
            //     [4] -> [3]; support: 3, confidence: 0.6 
            // };
        }
    }
}
