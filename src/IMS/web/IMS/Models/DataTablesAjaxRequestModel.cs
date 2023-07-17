using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IMS.Models
{
    public class DataTablesAjaxRequestModel
    {
        private readonly HttpRequestBase _request;

        public int Start
        {
            get
            {
                return int.Parse(_request["start"]);
            }
        }

        public int Length
        {
            get
            {
                return int.Parse(_request["length"]);
            }
        }

        public string SearchText
        {
            get
            {
                return _request["search[value]"];
            }
        }

        public string HeaderText
        {
            get
            {
                return _request.Headers["id"];
            }
        }

        public DataTablesAjaxRequestModel(HttpRequestBase request)
        {
            _request = request;
        }

        public string SortColumn
        {
            get
            {
                return _request["columns[" + _request["order[0][column]"] + "][name]"];
            }
        }

        public int SortColumnIndex
        {
            get
            {
                return int.Parse(_request.QueryString["iSortCol_0"]);
            }
        }

        public string SortDirection
        {
            get
            {
                return _request["order[0][dir]"];
            }
        }

        public int PageIndex
        {
            get
            {
                if (Length > 0)
                    return (Start / Length) + 1;
                else
                    return 1;
            }
        }

        public int PageSize
        {
            get
            {
                if (Length == 0)
                    return 1;
                else
                    return Length;
            }
        }

        public static object EmptyResult
        {
            get
            {
                return new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = (new string[] { }).ToArray()
                };
            }
        }

        public string GetSortText(string[] columnNames)
        {
            var method = _request.HttpMethod.ToLower();

            var queryString = _request.QueryString.AllKeys
                .Select(x => new KeyValuePair<string, StringValues>(x.ToString(), _request.QueryString[x.ToString()]));
            var queryForm = _request.Form.AllKeys
                .Select(x => new KeyValuePair<string, StringValues>(x.ToString(), _request.QueryString[x.ToString()]));

            if (method == "get")
                return ReadValues(queryString, columnNames);
            else if (method == "post")
                return ReadValues(queryForm, columnNames);
            else
                throw new InvalidOperationException("Http method not supported, use get or post");
        }

        private string ReadValues(IEnumerable<KeyValuePair<string, StringValues>>
            requestValues, string[] columnNames)
        {
            var sortText = new StringBuilder();
            for (var i = 0; i < columnNames.Length; i++)
            {
                if (requestValues.Any(x => x.Key == $"order[{i}][column]"))
                {
                    if (sortText.Length > 0)
                        sortText.Append(",");

                    var columnValue = requestValues.Where(x => x.Key == $"order[{i}][column]").FirstOrDefault();
                    var directionValue = requestValues.Where(x => x.Key == $"order[{i}][dir]").FirstOrDefault();

                    var column = int.Parse(columnValue.Value.ToArray()[0]);
                    var direction = directionValue.Value.ToArray()[0];
                    var sortDirection = $"{columnNames[column]} {(direction == "asc" ? "asc" : "desc")}";
                    sortText.Append(sortDirection);
                }
            }
            return sortText.ToString();
        }
    }
}