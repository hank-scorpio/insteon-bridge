var API =
{
	module: function (name, decl)
	{
		var m = angular.module(name, []);
		mapObject(m, '', '', decl);
	},
};

function mapObject(module, baseRoute, route, obj)
{

	if (baseRoute != '') route = baseRoute + '/' + route;

	_.forEach(_.pairs(obj), function (kv)
	{
		if (_.isArray(kv[1])) mapCollection(module, route, kv[0]);
		mapObject(module, route, kv[0], kv[1]);

	});
}

function mapCollection(module, route, name)
{
	module.controller(name + 'Controller',
	[
		'$scope', '$http', function ($scope, $http)
		{
			var IdProp = 'Id';

			$scope.Items = [];
			$scope.ItemsBy = function (prop)
			{
				return _.groupBy($scope.Items, prop);
			};
			$scope.Update = function (item, action)
			{
				var id = _.isString(item) ? item : _.result(item, IdProp);
		
				var url = route + '/' + name + '/' + id + '/' + (action || '');
				
				$http.get(url).success(function (items)
				{
					$scope.Items = _.uniq(_.union(items, $scope.Items), IdProp);
				});
			};

			$scope.Update('all', '');
		}
	]);
}



