using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facets.Core.Passes.DTOs;
public sealed record VisitorPassTemplateDto(decimal Width, 
                                            decimal Height, 
                                            string Template, 
                                            string PassCategoryColor, 
                                            string PassCategoryName, 
                                            string ProfileImage,
                                            string FullName);